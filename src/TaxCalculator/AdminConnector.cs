﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonUtils;
using PensionCoach.Tools.PostOpenApi;
using PensionCoach.Tools.PostOpenApi.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Data;
using Tax.Data.Abstractions;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator;

public class AdminConnector : IAdminConnector
{
    private readonly MunicipalityDbContext municipalityDbContext;
    private readonly ITaxDataPopulateService populateTaxDataService;
    private readonly IPostOpenApiClient postOpenApiClient;
    private readonly ILogger<MunicipalityConnector> logger;

    public AdminConnector(
        MunicipalityDbContext municipalityDbContext,
        ITaxDataPopulateService populateTaxDataService,
        IPostOpenApiClient postOpenApiClient,
        ILogger<MunicipalityConnector> logger)
    {
        this.municipalityDbContext = municipalityDbContext;
        this.populateTaxDataService = populateTaxDataService;
        this.postOpenApiClient = postOpenApiClient;
        this.logger = logger;
    }

    private int TotalCount { get; set; }

    public Task<int> PopulateWithTaxLocationAsync(bool doClear)
    {
        return populateTaxDataService.PopulateWithTaxLocationAsync(doClear);
    }

    public async Task<int> StagePlzTableAsync()
    {
        municipalityDbContext.TruncateTaxMunicipalityTable();

        int count = 0;

        await foreach (var model in GetAllZipCodesAsync(int.MaxValue))
        {
            municipalityDbContext.Add(new ZipEntity
            {
                BfsNumber = model.BfsCode,
                Canton = model.Canton,
                ZipCode = model.ZipCode,
                ZipCodeAddOn = model.ZipCodeAddOn,
                Name = model.MunicipalityName,
                LanguageCode = model.LanguageCode,
                DateOfValidity = model.DateOfValidity,
            });

            count++;
        }

        await municipalityDbContext.SaveChangesAsync(CancellationToken.None);

        return count;
    }

    /// <param name="limit"></param>
    /// <inheritdoc />
    public async IAsyncEnumerable<ZipModel> GetAllZipCodesAsync(int limit)
    {
        const int numberOfReaders = 5;
        const int limitPerFetch = 100;

        using (new MeasureTime(t => logger.LogDebug($"Execution time to fetch all ZIP: {t}ms")))
        {
            logger.LogDebug($"First fetch zip data ({limitPerFetch} items)");

            OpenApiZipInfo openApiData = await postOpenApiClient.GetZipCodesAsync(limitPerFetch, 0);

            if (openApiData is not { TotalCount: > 0 })
            {
                yield break;
            }

            TotalCount = openApiData.TotalCount;

            Channel<(int, int)> fetchZipChannel = Channel.CreateBounded<(int, int)>(new BoundedChannelOptions(5));
            Channel<ZipModel> resultZipChannel = Channel.CreateUnbounded<ZipModel>();

            Task[] consumers = Enumerable
                .Range(1, numberOfReaders)
                .Select(counter => ConsumeDataAsync(fetchZipChannel.Reader, resultZipChannel.Writer, counter))
                .ToArray();

            await WriteOpenApiDataToChannel(resultZipChannel, openApiData);

            await Task.Run(async () =>
            {
                int count = 1;
                while (count * limitPerFetch < TotalCount)
                {
                    await fetchZipChannel.Writer.WriteAsync((limitPerFetch, count * limitPerFetch));
                    count++;
                }

                fetchZipChannel.Writer.Complete();
            });

            await Task.WhenAll(consumers);

            resultZipChannel.Writer.Complete();

            await foreach (ZipModel record in resultZipChannel.Reader.ReadAllAsync(CancellationToken.None))
            {
                yield return record;
            }
        }
    }

    private async Task ConsumeDataAsync(ChannelReader<(int, int)> channelReader, ChannelWriter<ZipModel> channelWriter, int readerId)
    {
        while (await channelReader.WaitToReadAsync())
        {
            if (channelReader.TryRead(out (int limit, int offset) fetch))
            {
                logger.LogDebug($"Fetch zip data by reader {readerId}: limit {fetch.limit}, offset {fetch.offset}");

                OpenApiZipInfo openApiData = await postOpenApiClient.GetZipCodesAsync(fetch.limit, fetch.offset);

                if (openApiData is null || openApiData.Records.Length() == 0)
                {
                    break;
                }

                await WriteOpenApiDataToChannel(channelWriter, openApiData);
            }
        }

        logger.LogDebug($"Reader {readerId} completed");
    }

    private static async Task WriteOpenApiDataToChannel(ChannelWriter<ZipModel> channelWriter, OpenApiZipInfo openApiData)
    {
        foreach (ZipModel model in openApiData.Records.Select(x => new ZipModel
                 {
                     BfsCode = x.Record.Fields.BfsCode,
                     MunicipalityName = x.Record.Fields.MunicipalityName,
                     Canton = x.Record.Fields.Canton,
                     ZipCode = x.Record.Fields.ZipCode,
                     ZipCodeAddOn = x.Record.Fields.ZipCodeAddOn,
                     LanguageCode = x.Record.Fields.LanguageCode,
                     DateOfValidity = DateTime.Parse(x.Record.Fields.DateOfValidity, CultureInfo.InvariantCulture),
                 }))
        {
            await channelWriter.WriteAsync(model);
        }
    }
}

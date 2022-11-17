using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;
using Tax.Data.Abstractions;
using Tax.Data.Abstractions.Models;
using Tax.Data.Abstractions.Models.Populate;

namespace Tax.Data;

public class StaticTaxDataPopulateService : ITaxDataPopulateService
{
    private readonly MunicipalityDbContext municipalityDbContext;
    private readonly IEstvTaxCalculatorClient estvTaxCalculatorClient;

    public StaticTaxDataPopulateService(
        MunicipalityDbContext municipalityDbContext, IEstvTaxCalculatorClient estvTaxCalculatorClient)
    {
        this.municipalityDbContext = municipalityDbContext;
        this.estvTaxCalculatorClient = estvTaxCalculatorClient;
    }

    public async Task<int> PopulateWithTaxLocationAsync(bool doClear)
    {
        const int numberOfReaders = 5;
        const int numberOfFetchLevels = 6;

        if (doClear)
        {
            foreach (var municipalityEntity in municipalityDbContext.MunicipalityEntities.AsNoTracking())
            {
                municipalityEntity.TaxLocationId = null;
                municipalityEntity.Remark = null;
                municipalityDbContext.Update(municipalityEntity);
            }

            await municipalityDbContext.SaveChangesAsync(CancellationToken.None);

            municipalityDbContext.ChangeTracker.Clear();
        }

        var fetchLevelChannels = new Channel<TaxLocationSearchHolder>[numberOfFetchLevels];

        for (int i = 0; i < fetchLevelChannels.Length; i++)
        {
            fetchLevelChannels[i] = Channel.CreateUnbounded<TaxLocationSearchHolder>();
        }

        Channel<TaxLocationSearchHolder> dbStoreChannel = Channel.CreateUnbounded<TaxLocationSearchHolder>();

        Dictionary<int, Func<MunicipalityEntity, Task<TaxLocation[]>>> clientApiFetcher =
            new Dictionary<int, Func<MunicipalityEntity, Task<TaxLocation[]>>>
            {
                { 0, entity => SearchApi(new SearchMunicipalityRequest(entity.ZipCode, entity.CleanName)) },
                { 1, entity => SearchApi(new SearchMunicipalityRequest(string.Empty, entity.CleanName)) },
                { 2, entity => SearchApi(new SearchMunicipalityRequest(entity.ZipCode, GetSplitName(entity))) },
                { 3, entity => SearchApi(new SearchMunicipalityRequest("", GetSplitName(entity))) },
                { 4, entity => SearchApi(new SearchMunicipalityRequest(entity.ZipCode, string.Empty)) },
                { 5, entity => SearchApi(new SearchMunicipalityRequest("", entity.OverruledName)) },
            };

        Task[] fetchConsumers0 = Enumerable
            .Range(1, numberOfReaders)
            .Select(_ => FetchConsumerAsync(fetchLevelChannels[0], fetchLevelChannels[1], clientApiFetcher[0]))
            .ToArray();

        Task[] fetchConsumers1 = Enumerable
            .Range(1, numberOfReaders)
            .Select(_ => FetchConsumerAsync(fetchLevelChannels[1], fetchLevelChannels[2], clientApiFetcher[1]))
            .ToArray();

        Task[] fetchConsumers2 = Enumerable
            .Range(1, numberOfReaders)
            .Select(_ => FetchConsumerAsync(fetchLevelChannels[2], fetchLevelChannels[3], clientApiFetcher[2]))
            .ToArray();

        Task[] fetchConsumers3 = Enumerable
            .Range(1, numberOfReaders)
            .Select(_ => FetchConsumerAsync(fetchLevelChannels[3], fetchLevelChannels[4], clientApiFetcher[3]))
            .ToArray();

        Task[] fetchConsumers4 = Enumerable
            .Range(1, numberOfReaders)
            .Select(_ => FetchConsumerAsync(fetchLevelChannels[4], fetchLevelChannels[5], clientApiFetcher[4]))
            .ToArray();

        Task[] fetchConsumers5 = Enumerable
            .Range(1, numberOfReaders)
            .Select(_ => FetchConsumerAsync(fetchLevelChannels[5], dbStoreChannel, clientApiFetcher[5]))
            .ToArray();

        await Task.Run(async () => await FillInitialStage(fetchLevelChannels[0]));

        fetchLevelChannels[0].Writer.Complete();
        await Task.WhenAll(fetchConsumers0);

        fetchLevelChannels[1].Writer.Complete();
        await Task.WhenAll(fetchConsumers1);

        fetchLevelChannels[2].Writer.Complete();
        await Task.WhenAll(fetchConsumers2);

        fetchLevelChannels[3].Writer.Complete();
        await Task.WhenAll(fetchConsumers3);

        fetchLevelChannels[4].Writer.Complete();
        await Task.WhenAll(fetchConsumers4);

        fetchLevelChannels[5].Writer.Complete();
        await Task.WhenAll(fetchConsumers5);

        dbStoreChannel.Writer.Complete();

        return await StoreResults(dbStoreChannel);

        async Task FetchConsumerAsync(
            Channel<TaxLocationSearchHolder> fetchChannel,
            Channel<TaxLocationSearchHolder> successorChannel,
            Func<MunicipalityEntity, Task<TaxLocation[]>> searcher)
        {
            while (await fetchChannel.Reader.WaitToReadAsync())
            {
                if (!fetchChannel.Reader.TryRead(out TaxLocationSearchHolder data))
                {
                    continue;
                }

                if (data.SearchResultType == SearchResultType.Match)
                {
                    await successorChannel.Writer.WriteAsync(data);
                }
                else
                {
                    TaxLocation[] locations = await searcher(data.MunicipalityEntity);

                    TaxLocationSearchHolder searchResult = Dispatch(data.SearchLevel, locations, data.MunicipalityEntity);

                    await successorChannel.Writer.WriteAsync(searchResult);
                }
            }
        }
    }

    private TaxLocationSearchHolder Dispatch(int searchLevel, TaxLocation[] taxLocations, MunicipalityEntity municipalityEntity)
    {
        TaxLocationSearchHolder searchHolder = CreateSearchHolder(taxLocations, municipalityEntity);
        searchHolder.SearchLevel = searchLevel;
        searchHolder.SearchResultType = SearchResultType.NotSet;

        (bool match, TaxLocation[] locations) = ApplyCheckers(searchHolder);

        if (match && locations.Length == 1)
        {
            return searchHolder with
            {
                SearchResultType = SearchResultType.Match,
                TaxLocations = locations
            };
        }

        return searchHolder;
    }

    private TaxLocationSearchHolder CreateSearchHolder(TaxLocation[] locations, MunicipalityEntity entity)
    {
        if (locations.Length > 1)
        {
            return new TaxLocationSearchHolder
            {
                MunicipalityEntity = entity,
                TaxLocations = locations,
                SearchResultType = SearchResultType.NotUnique,
                SearchLevel = 1,
            };
        }

        if (locations.Length == 0)
        {
            return new TaxLocationSearchHolder
            {
                MunicipalityEntity = entity,
                SearchResultType = SearchResultType.None,
                SearchLevel = 1,
            };
        }

        return new TaxLocationSearchHolder
        {
            MunicipalityEntity = entity,
            SearchResultType = SearchResultType.Match,
            TaxLocations = locations,
        };
    }

    private async Task FillInitialStage(Channel<TaxLocationSearchHolder> fetchChannel)
    {
        foreach (MunicipalityEntity model in municipalityDbContext.MunicipalityEntities
                     .Where(item => item.SuccessorId == 0 &&
                                    item.MutationType == 11 &&
                                    item.TaxLocationId == null))
        {
            await fetchChannel.Writer.WriteAsync(
                new TaxLocationSearchHolder
                {
                    SearchLevel = 0,
                    MunicipalityEntity = model,
                    TaxLocations = Array.Empty<TaxLocation>()
                });
        }
    }

    private async Task<TaxLocation[]> SearchApi(SearchMunicipalityRequest searchRequest)
    {
        if (searchRequest is null || (searchRequest.Zipcode == "" && searchRequest.Name == ""))
        {
            return Array.Empty<TaxLocation>();
        }

        return await estvTaxCalculatorClient.GetTaxLocationsAsync(searchRequest.Zipcode, searchRequest.Name) switch
        {
            null => Array.Empty<TaxLocation>(),
            { } a => a
        };
    }

    private (bool, TaxLocation[]) ApplyCheckers(TaxLocationSearchHolder searchHolder)
    {
        Func<TaxLocation[], MunicipalityEntity, TaxLocation[]>[] checkers =
        {
            MatchZipAndName,
            MatchNameOnly,
            MatchCanton,
            MatchContainsNameAndCanton,
            MatchByZipAsTaxId,
            MatchByNamePart
        };

        if (searchHolder.TaxLocations == null)
        {
            return (false, null);
        }

        foreach (var checker in checkers)
        {
            TaxLocation[] filteredLocations = checker(searchHolder.TaxLocations, searchHolder.MunicipalityEntity);

            if (filteredLocations.Length == 1)
            {
                return (true, filteredLocations);
            }
        }

        return (false, searchHolder.TaxLocations);
    }

    private async Task<int> StoreResults(Channel<TaxLocationSearchHolder> dbStoreChannel)
    {
        int count = 0;
        await foreach (TaxLocationSearchHolder searchHolder in dbStoreChannel.Reader.ReadAllAsync(CancellationToken.None))
        {
            // not found
            if (searchHolder.SearchResultType is SearchResultType.None or
                SearchResultType.NotUnique or
                SearchResultType.NotSet)
            {
                searchHolder.MunicipalityEntity.Remark = searchHolder.SearchResultType.ToString();
                municipalityDbContext.Update(searchHolder.MunicipalityEntity);
                continue;
            }

            searchHolder.MunicipalityEntity.TaxLocationId = searchHolder.TaxLocations[0].Id;
            if (searchHolder.MunicipalityEntity.ZipCode is null && searchHolder.TaxLocations[0].ZipCode is not null)
            {
                searchHolder.MunicipalityEntity.ZipCode = searchHolder.TaxLocations[0].ZipCode;
            }

            municipalityDbContext.Update(searchHolder.MunicipalityEntity);

            count++;
        }

        await municipalityDbContext.SaveChangesAsync(CancellationToken.None);

        return count;
    }

    private TaxLocation[] MatchZipAndName(TaxLocation[] sourceLocations, MunicipalityEntity entity)
    {
        if (entity is null)
        {
            return Array.Empty<TaxLocation>();
        }

        return (sourceLocations ?? Array.Empty<TaxLocation>())
            .Where(item => item.ZipCode == entity.ZipCode && item.City == entity.CleanName)
            .ToArray();
    }

    private TaxLocation[] MatchNameOnly(TaxLocation[] sourceLocations, MunicipalityEntity entity)
    {
        if (entity is null)
        {
            return Array.Empty<TaxLocation>();
        }

        return (sourceLocations ?? Array.Empty<TaxLocation>())
            .Where(item => item.City == entity.CleanName)
            .ToArray();
    }

    private TaxLocation[] MatchCanton(TaxLocation[] sourceLocations, MunicipalityEntity entity)
    {
        return (sourceLocations ?? Array.Empty<TaxLocation>())
            .Where(item => item.Canton == entity.Canton)
            .ToArray();
    }

    private TaxLocation[] MatchContainsNameAndCanton(TaxLocation[] sourceLocations, MunicipalityEntity entity)
    {
        return (sourceLocations ?? Array.Empty<TaxLocation>())
            .Where(item => item.City.Contains(entity.CleanName) && item.Canton == entity.Canton)
            .ToArray();
    }

    private TaxLocation[] MatchByZipAsTaxId(TaxLocation[] sourceLocations, MunicipalityEntity entity)
    {
        int taxIdFromZipCode = Convert.ToInt32(entity.ZipCode) * 100000;

        return (sourceLocations ?? Array.Empty<TaxLocation>())
            .Where(item => item.Id == taxIdFromZipCode)
            .ToArray();
    }

    private TaxLocation[] MatchByNamePart(TaxLocation[] sourceLocations, MunicipalityEntity entity)
    {
        // just try part of name before a hypen (ie. Illnau-Effretikon)
        return GetSplitName(entity) switch
        {
            { } p => Filter(p),
            _ => Array.Empty<TaxLocation>()
        };

        TaxLocation[] Filter(string part)
        {
            return (sourceLocations ?? Array.Empty<TaxLocation>())
                .Where(item => item.City == part)
                .ToArray();
        }
    }

    // just try part of name before a hypen (ie. Illnau-Effretikon)
    private string GetSplitName(MunicipalityEntity entity)
    {
        return entity.CleanName.Split('-', ' ', '/') switch
        {
            { Length: 2 } p => p[0],
            _ => null
        };
    }
}

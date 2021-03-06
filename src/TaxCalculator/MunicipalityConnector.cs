﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Municipality;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator
{
    public class MunicipalityConnector : IMunicipalityConnector
    {
        private readonly IMapper mapper;
        private readonly MunicipalityDbContext municipalityDbContext;
        private readonly Func<TaxRateDbContext> dbContext;

        public MunicipalityConnector(
            IMapper mapper,
            MunicipalityDbContext municipalityDbContext,
            Func<TaxRateDbContext> dbContext)
        {
            this.mapper = mapper;
            this.municipalityDbContext = municipalityDbContext;
            this.dbContext = dbContext;
        }

        public Task<IEnumerable<MunicipalityModel>> GetAllAsync()
        {
            return Task.FromResult(
                this.mapper.Map<IEnumerable<MunicipalityModel>>(
                    this.municipalityDbContext.MunicipalityEntities.ToList()));
        }

        /// <summary>
        /// Searches the specified search filter.
        /// </summary>
        /// <param name="searchFilter">The search filter.</param>
        /// <returns>List of municipalities.</returns>
        public IEnumerable<MunicipalityModel> Search(MunicipalitySearchFilter searchFilter)
        {
            IQueryable<MunicipalityEntity> result = this.municipalityDbContext.MunicipalityEntities;

            if (searchFilter.Canton != Canton.Undefined)
            {
                result = result.Where(item => item.Canton == searchFilter.Canton.ToString());
            }

            if (!string.IsNullOrEmpty(searchFilter.Name))
            {
                result =
                    result.Where(item => item.Name.Contains(searchFilter.Name));
            }

            foreach (MunicipalityEntity entity in result)
            {
                var model = this.mapper.Map<MunicipalityModel>(entity);

                if (searchFilter.YearOfValidity.HasValue)
                {
                    if (!model.DateOfMutation.HasValue)
                    {
                        yield return model;
                    }
                    else if (model.DateOfMutation.Value.Year > searchFilter.YearOfValidity)
                    {
                        yield return model;
                    }
                }
                else
                {
                    yield return model;
                }
            }
        }

        public Task<Either<string, MunicipalityModel>> GetAsync(
            int bfsNumber, int year)
        {
            Option<MunicipalityEntity> entity =
                this.municipalityDbContext.MunicipalityEntities
                    .FirstOrDefault(item => item.BfsNumber == bfsNumber
                                            && string.IsNullOrEmpty(item.DateOfMutation));

            return entity
                .Match<Either<string, MunicipalityModel>>(
                    Some: item => this.mapper.Map<MunicipalityModel>(item),
                    None: () => $"Municipality by ReligiousGroupType.Protestant {bfsNumber} not found")
                .AsTask();
        }

        public Task<IReadOnlyCollection<TaxSupportedMunicipalityModel>> GetAllSupportTaxCalculationAsync(int year)
        {
            using (var ctx = this.dbContext())
            {
                IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities =
                    ctx.Rates
                        .Where(
                            item => item.Year == year)
                        .OrderBy(item => item.MunicipalityName)
                        .Select(item => new TaxSupportedMunicipalityModel
                        {
                            BfsNumber = item.BfsId,
                            Name = item.MunicipalityName,
                            Canton = Enum.Parse<Canton>(item.Canton),
                        })
                        .ToList();

                return Task.FromResult(municipalities);
            }
        }
    }
}
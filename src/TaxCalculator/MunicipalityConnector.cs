using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator
{
    public class MunicipalityConnector : IMunicipalityConnector
    {
        private readonly IMapper mapper;
        private readonly MunicipalityDbContext municipalityDbContext;

        public MunicipalityConnector(
            IMapper mapper, MunicipalityDbContext municipalityDbContext)
        {
            this.mapper = mapper;
            this.municipalityDbContext = municipalityDbContext;
        }

        public Task<IEnumerable<MunicipalityModel>> GetAllAsync()
        {
            return Task.FromResult(
                this.mapper.Map<IEnumerable<MunicipalityModel>>(
                    this.municipalityDbContext.MunicipalityEntities.ToList()));
        }

        public Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber)
        {
            Option<MunicipalityEntity> entity =
                this.municipalityDbContext.MunicipalityEntities
                    .FirstOrDefault(item => item.BfsNumber == bfsNumber);

            return entity
                .Match<Either<string, MunicipalityModel>>(
                    Some: item => this.mapper.Map<MunicipalityModel>(item),
                    None: () => $"Municipality by BFS number {bfsNumber} not found")
                .AsTask();
        }
    }
}
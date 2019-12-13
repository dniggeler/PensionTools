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
        private readonly Func<MunicipalityDbContext> municipalityDbContextFunc;

        public MunicipalityConnector(IMapper mapper, Func<MunicipalityDbContext> municipalityDbContextFunc)
        {
            this.mapper = mapper;
            this.municipalityDbContextFunc = municipalityDbContextFunc;
        }

        public Task<IEnumerable<MunicipalityModel>> GetAllAsync()
        {
            using (var ctxt = this.municipalityDbContextFunc())
            {
                return Task.FromResult(
                    this.mapper.Map<IEnumerable<MunicipalityModel>>(ctxt.MunicipalityEntities.ToList()));
            }
        }

        public Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber)
        {
            using (var ctxt = this.municipalityDbContextFunc())
            {
                Option<MunicipalityEntity> entity =
                    ctxt.MunicipalityEntities
                        .FirstOrDefault(item => item.BfsNumber == bfsNumber);

                return entity
                    .Match<Either<string, MunicipalityModel>>(
                        Some: item => this.mapper.Map<MunicipalityModel>(item),
                        None: () => $"Municipality by BFS number {bfsNumber} not found")
                    .AsTask();
            }
        }
    }
}
using Application.Municipality;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Estv;

public class EstvMunicipalityConnector : IMunicipalityConnector
{
    private readonly IMapper mapper;
    private readonly IMunicipalityRepository municipalityRepository;

    public EstvMunicipalityConnector(
        IMapper mapper,
        IMunicipalityRepository municipalityRepository)
    {
        this.mapper = mapper;
        this.municipalityRepository = municipalityRepository;
    }

    public Task<IEnumerable<MunicipalityModel>> GetAllAsync()
    {
        return Task.FromResult(
            mapper.Map<IEnumerable<MunicipalityModel>>(municipalityRepository.GetAll()));
    }

    /// <summary>
    /// Searches the specified search filter.
    /// </summary>
    /// <param name="searchFilter">The search filter.</param>
    /// <returns>List of municipalities.</returns>
    public IEnumerable<MunicipalityModel> Search(MunicipalitySearchFilter searchFilter)
    {
        foreach (MunicipalityEntity entity in municipalityRepository.Search(searchFilter))
        {
            var model = mapper.Map<MunicipalityModel>(entity);

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

    public Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber, int year)
    {
        Option<MunicipalityEntity> entity =
            municipalityRepository.Get(bfsNumber, year);

        return entity
            .Match<Either<string, MunicipalityModel>>(
                Some: item => mapper.Map<MunicipalityModel>(item),
                None: () => $"Municipality not found by BFS number {bfsNumber}")
            .AsTask();
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<TaxSupportedMunicipalityModel>> GetAllSupportTaxCalculationAsync()
    {
        const int maxEstvSupportedYear = 2022;

        IReadOnlyCollection<TaxSupportedMunicipalityModel> list = municipalityRepository
            .GetAllSupportTaxCalculation()
            .Select(item => new TaxSupportedMunicipalityModel
            {
                BfsMunicipalityNumber = item.BfsNumber,
                Name = item.Name,
                Canton = Enum.Parse<Canton>(item.Canton),
                MaxSupportedYear = maxEstvSupportedYear,
                EstvTaxLocationId = item.TaxLocationId
            })
            .ToList();

        return Task.FromResult(list);
    }
}

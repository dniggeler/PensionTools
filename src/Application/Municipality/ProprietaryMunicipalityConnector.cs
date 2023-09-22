using Application.Tax.Proprietary.Abstractions.Repositories;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Municipality;

public class ProprietaryMunicipalityConnector : IMunicipalityConnector
{
    private readonly IMapper mapper;
    private readonly IMunicipalityRepository municipalityRepository;
    private readonly IStateTaxRateRepository stateTaxRateRepository;

    public ProprietaryMunicipalityConnector(
        IMapper mapper,
        IMunicipalityRepository municipalityRepository,
        IStateTaxRateRepository stateTaxRateRepository)
    {
        this.mapper = mapper;
        this.municipalityRepository = municipalityRepository;
        this.stateTaxRateRepository = stateTaxRateRepository;
    }

    public Task<IEnumerable<MunicipalityModel>> GetAllAsync()
    {
        return Task.FromResult(mapper.Map<IEnumerable<MunicipalityModel>>(municipalityRepository.GetAll()));
    }

    /// <summary>
    /// Searches the specified search filter.
    /// </summary>
    /// <param name="searchFilter">The search filter.</param>
    /// <returns>List of municipalities.</returns>
    public IEnumerable<MunicipalityModel> Search(MunicipalitySearchFilter searchFilter)
    {
        foreach (var entity in municipalityRepository.Search(searchFilter))
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
        Option<MunicipalityEntity> entity = municipalityRepository.GetAll()
                .FirstOrDefault(item => item.BfsNumber == bfsNumber
                                        && string.IsNullOrEmpty(item.DateOfMutation));

        return entity
            .Match<Either<string, MunicipalityModel>>(
                Some: item => mapper.Map<MunicipalityModel>(item),
                None: () => $"Municipality not found by BFS number {bfsNumber}")
            .AsTask();
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<TaxSupportedMunicipalityModel>> GetAllSupportTaxCalculationAsync()
    {
        IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities =
            stateTaxRateRepository.TaxRates()
                .AsEnumerable()
                .GroupBy(keySelector => new
                {
                    Id = keySelector.BfsId,
                    Name = keySelector.MunicipalityName,
                    keySelector.Canton,
                })
                .Select(item => new TaxSupportedMunicipalityModel
                {
                    BfsMunicipalityNumber = item.Key.Id,
                    Name = item.Key.Name,
                    Canton = Enum.Parse<Canton>(item.Key.Canton),
                    MaxSupportedYear = item.Max(entity => entity.Year),
                })
                .OrderBy(item => item.Name)
                .ToList();

        return Task.FromResult(municipalities);
    }
}

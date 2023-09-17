using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Municipality;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Proprietary;

namespace PensionCoach.Tools.TaxCalculator.Mock;

public class MockedFullTaxCalculator
    : IFullWealthAndIncomeTaxCalculator, IFullCapitalBenefitTaxCalculator, IMunicipalityConnector, ITaxSupportedYearProvider
{
    const int DefaultBfsMunicipalityId = 261;
    const Canton DefaultCanton = Canton.ZH;

    private readonly ProprietaryFullTaxCalculator fullTaxCalculator;
    private readonly ProprietaryFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator;

    public MockedFullTaxCalculator(
        ProprietaryFullTaxCalculator fullTaxCalculator,
        ProprietaryFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator)
    {
        this.fullTaxCalculator = fullTaxCalculator;
        this.fullCapitalBenefitTaxCalculator = fullCapitalBenefitTaxCalculator;
    }

    public Task<Either<string, FullTaxResult>> CalculateAsync(
        int calculationYear, MunicipalityModel municipality, TaxPerson person, bool withMaxAvailableCalculationYear = false)
    {
        MunicipalityModel adaptedModel = GetAdaptedModel();

        return fullTaxCalculator.CalculateAsync(calculationYear, adaptedModel, person, withMaxAvailableCalculationYear);
    }

    public Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear, MunicipalityModel municipality, CapitalBenefitTaxPerson person, bool withMaxAvailableCalculationYear = false)
    {
        MunicipalityModel adaptedModel = GetAdaptedModel();

        return fullCapitalBenefitTaxCalculator.CalculateAsync(calculationYear, adaptedModel, person, withMaxAvailableCalculationYear);
    }

    public Task<IEnumerable<MunicipalityModel>> GetAllAsync()
    {
        return Search(null).AsTask();
    }

    public IEnumerable<MunicipalityModel> Search(MunicipalitySearchFilter searchFilter)
    {
        yield return GetAdaptedModel();
    }

    public Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber, int year)
    {
        Either<string, MunicipalityModel> municipality = GetAdaptedModel();

        return municipality.AsTask();
    }

    public Task<IReadOnlyCollection<TaxSupportedMunicipalityModel>> GetAllSupportTaxCalculationAsync()
    {
        MunicipalityModel adaptedModel = GetAdaptedModel();

        IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities = new List<TaxSupportedMunicipalityModel>
        {
            new() { MaxSupportedYear = 2022, BfsMunicipalityNumber = adaptedModel.BfsNumber, Canton = adaptedModel.Canton }
        };

        return municipalities.AsTask();
    }

    public int[] GetSupportedTaxYears()
    {
        int[] years = { 2022, 2023 };

        return years;
    }

    public int MapToSupportedYear(int taxYear)
    {
        return GetSupportedTaxYears().Max();
    }

    private MunicipalityModel GetAdaptedModel()
    {
        return new MunicipalityModel { BfsNumber = DefaultBfsMunicipalityId, Canton = DefaultCanton };
    }
}

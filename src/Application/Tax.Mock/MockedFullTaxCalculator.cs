using Application.Features.FullTaxCalculation;
using Application.Municipality;
using Application.Tax.Contracts;
using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Enums;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Mock
{
    public class MockedFullTaxCalculator
        : IFullWealthAndIncomeTaxCalculator, IFullCapitalBenefitTaxCalculator, IMunicipalityConnector, ITaxSupportedYearProvider
    {
        const int DefaultBfsMunicipalityId = 261;
        const Canton DefaultCanton = Canton.ZH;

        public async Task<Either<string, FullTaxResult>> CalculateAsync(
            int calculationYear, MunicipalityModel municipality, TaxPerson person, bool withMaxAvailableCalculationYear = false)
        {
            MunicipalityModel adaptedModel = GetAdaptedModel();

            var result = new FullTaxResult
            {
                FederalTaxResult = new BasisTaxResult
                {
                    TaxAmount = 1200,
                    DeterminingFactorTaxableAmount = 0.05M
                },

                StateTaxResult = new StateTaxResult
                {
                    BasisIncomeTax = new BasisTaxResult
                    {
                        TaxAmount = 1000,
                        DeterminingFactorTaxableAmount = 0.04M
                    },

                    BasisWealthTax = new BasisTaxResult
                    {
                        TaxAmount = 500,
                        DeterminingFactorTaxableAmount = 0.03M
                    },
                    ChurchTax = new ChurchTaxResult
                    {
                        TaxAmount = 100,
                    }
                }
            };

            return await result.AsTask();
        }

        public async Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
            int calculationYear, MunicipalityModel municipality, CapitalBenefitTaxPerson person, bool withMaxAvailableCalculationYear = false)
        {
            MunicipalityModel adaptedModel = GetAdaptedModel();

            var result = new FullCapitalBenefitTaxResult
            {
                FederalResult = new BasisTaxResult
                {
                    TaxAmount = 100,
                    DeterminingFactorTaxableAmount = 0.02M,
                },
                StateResult = new CapitalBenefitTaxResult
                {
                    BasisTax = new BasisTaxResult
                    {
                        TaxAmount = 500,
                        DeterminingFactorTaxableAmount = 0.03M,
                    },
                    ChurchTax = new ChurchTaxResult
                    {
                        TaxAmount = 50,
                        TaxAmountPartner = 0,
                        TaxRate = 0.01M,
                    }
                }
            };

            return await Task.FromResult(result);
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
}

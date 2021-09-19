using System.Collections.Generic;
using Calculators.CashFlow;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;
using Swashbuckle.AspNetCore.Filters;
using TaxCalculator.WebApi.Models;

namespace TaxCalculator.WebApi.Examples
{
    public class MultiPeriodRequestExample : IExamplesProvider<MultiPeriodRequest>
    {
        public MultiPeriodRequest GetExamples()
        {
            string personName = "Test Person";

            int startingYear = 2021;
            int finalYear = 2030;
            int numberOfPeriods = finalYear - startingYear + 1;

            return new MultiPeriodRequest
            {
                Name = "Swagger Sample",
                StartingYear = startingYear,
                NumberOfPeriods = numberOfPeriods,
                BfsMunicipalityId = 261,
                CivilStatus = CivilStatus.Married,
                Income = 100_000,
                Wealth = 500_000,
                CapitalBenefitsPillar3A = 100_000,
                CapitalBenefitsPension = 400_000,
                ReligiousGroupType = ReligiousGroupType.Other,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                CashFlowDefinitionHolder = new CashFlowDefinitionHolder
                {
                    ClearCashFlowDefinitions = new List<ClearActionDefinition>
                    {
                        new ()
                        {
                            Id = "Clear Capital Benefit Action",
                            Name = $"{personName} - Clear Capital Benefit Action",
                            ClearAtYear = finalYear,
                            ClearRatio = 1.0M,
                            Flow = new FlowPair(AccountType.CapitalBenefits, AccountType.Wealth),
                            IsTaxable = true,
                            TaxType = TaxType.Capital,
                            OccurrenceType = OccurrenceType.EndOfPeriod,
                        },
                    },
                    GenericCashFlowDefinitions = new List<GenericCashFlowDefinition>
                    {
                        new ()
                        {
                            Id = "my 3a account",
                            Name = $"{personName} - 3a Pillar",
                            InitialAmount = 6883,
                            RecurringInvestment = new RecurringInvestment(6883, FrequencyType.Yearly),
                            Flow = new FlowPair(AccountType.Income, AccountType.CapitalBenefits),
                            InvestmentPeriod = new InvestmentPeriod(startingYear, numberOfPeriods),
                            IsTaxable = false,
                            TaxType = TaxType.Undefined,
                            OccurrenceType = OccurrenceType.BeginOfPeriod,
                        },
                        new ()
                        {
                            Id = "my PK account",
                            NetGrowthRate = 0,
                            Name = "PK-Einkauf",
                            InitialAmount = 10000,
                            RecurringInvestment = new RecurringInvestment(10000, FrequencyType.Yearly),
                            Flow = new FlowPair(AccountType.Income, AccountType.CapitalBenefits),
                            InvestmentPeriod = new InvestmentPeriod(startingYear, 5),
                            IsTaxable = false,
                            TaxType = TaxType.Undefined,
                            OccurrenceType = OccurrenceType.BeginOfPeriod,
                        },
                    },
                },
            };
        }
    }
}

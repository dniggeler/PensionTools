using System;
using System.Collections.Generic;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
using PensionCoach.Tools.CommonTypes.Tax;
using Swashbuckle.AspNetCore.Filters;

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
                CapitalBenefitsPillar3A = 0,
                CapitalBenefitsPension = 0,
                ReligiousGroupType = ReligiousGroupType.Other,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                CashFlowDefinitionHolder = new CashFlowDefinitionHolder
                {
                    ClearAccountActions = new List<ClearAccountAction>
                    {
                        new ()
                        {
                            Id = "Clear Capital Benefit Action",
                            Name = $"{personName} - Clear Capital Benefit Action",
                            DateOfClearing = new DateTime(finalYear, 1, 1),
                            ClearRatio = 1.0M,
                            Flow = new FlowPair(AccountType.OccupationalPension, AccountType.Wealth),
                            IsTaxable = true,
                            TaxType = TaxType.CapitalBenefits,
                            OccurrenceType = OccurrenceType.EndOfPeriod,
                        },
                    },

                    ChangeResidenceActions = new List<ChangeResidenceAction>
                    {
                        new ()
                        {
                            Id = "Change residence",
                            Name = $"{personName} - Change Residence",
                            DestinationMunicipalityId = 3426,
                            DestinationCanton = Canton.SG,
                            ChangeCost = 5_000,
                            DateOfChange = new DateTime(2029, 7, 1)
                        },
                    },

                    GenericCashFlowDefinitions = new List<GenericCashFlowDefinition>
                    {
                        new ()
                        {
                            Header = new CashFlowHeader
                            {
                                Id = "my 3a account",
                                Name = $"{personName} - 3a Pillar"
                            },

                            InitialAmount = 100_000,
                            RecurringInvestment = new RecurringInvestment
                            {
                                Amount = 6883,
                                Frequency = FrequencyType.Yearly,
                            },
                            Flow = new FlowPair(AccountType.Income, AccountType.ThirdPillar),
                            InvestmentPeriod = new InvestmentPeriod
                            {
                                Year = startingYear,
                                NumberOfPeriods = numberOfPeriods,
                            },
                            IsTaxable = false,
                            TaxType = TaxType.Undefined,
                            OccurrenceType = OccurrenceType.BeginOfPeriod,
                        },
                        new ()
                        {
                            Header = new CashFlowHeader
                            {
                                Id = "my PK account",
                                Name = "PK-Einkauf",
                            },

                            NetGrowthRate = 0,
                            InitialAmount = 400_000,
                            RecurringInvestment = new RecurringInvestment
                            {
                                Amount = 10000,
                                Frequency = FrequencyType.Yearly,
                            },
                            Flow = new FlowPair(AccountType.Income, AccountType.OccupationalPension),
                            InvestmentPeriod = new InvestmentPeriod
                            {
                                Year = startingYear,
                                NumberOfPeriods = 5,
                            },
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

using System;
using System.Collections.Generic;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxComparison;
using Swashbuckle.AspNetCore.Filters;

namespace TaxCalculator.WebApi.Examples;

public class CapitalBenefitTransferInRequestExample : IExamplesProvider<CapitalBenefitTransferInComparerRequest>
{
    public CapitalBenefitTransferInComparerRequest GetExamples()
    {
        return new CapitalBenefitTransferInComparerRequest
        {
            Name = "Test",
            CalculationYear = 2022,
            CivilStatus = CivilStatus.Married,
            BfsMunicipalityId = 261,
            ReligiousGroup = ReligiousGroupType.Other,
            PartnerReligiousGroup = ReligiousGroupType.Other,
            TaxableIncome = 150_000M,
            TaxableFederalIncome = 140_000M,
            TaxableWealth = 500_000,
            WithCapitalBenefitTaxation = true,
            NetReturnRate = 0.01M,
            YearOfCapitalBenefitWithdrawal = 2032,
            FinalRetirementCapital = 800_000,
            TransferIns = new List<SingleTransferInModel>
            {
                new(10_000, new DateTime(2022, 1, 1))
            }
        };
    }
}

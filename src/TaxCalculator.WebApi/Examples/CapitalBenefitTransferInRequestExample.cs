using System;
using System.Collections.Generic;
using Domain.Enums;
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
            NetWealthReturn = 0.01M,
            CapitalBenefitsBeforeWithdrawal = 800_000,
            TransferIns = new List<SingleTransferInModel>
            {
                new(15_000, new DateTime(2022, 1, 1))
            },
            Withdrawals = new List<SingleTransferInModel>
            {
                new(0.5M, new DateTime(2032, 12, 31)),
                new(1, new DateTime(2033, 12, 31))
            }
        };
    }
}

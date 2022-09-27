﻿using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxComparison;
using Swashbuckle.AspNetCore.Filters;
using TaxCalculator.WebApi.Models;

namespace TaxCalculator.WebApi.Examples;

public class CapitalBenefitTaxComparerRequestExample : IExamplesProvider<CapitalBenefitTaxComparerRequest>
{
    public CapitalBenefitTaxComparerRequest GetExamples()
    {
        return new CapitalBenefitTaxComparerRequest
        {
            Name = "Test",
            CivilStatus = CivilStatus.Single,
            ReligiousGroup = ReligiousGroupType.Other,
            TaxableBenefits = 1000_000,
            BfsNumberList = new []{ 261, 24, 3 }
        };
    }
}
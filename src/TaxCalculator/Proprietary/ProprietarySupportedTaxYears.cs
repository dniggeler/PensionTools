﻿using System.Linq;
using Application.Features.FullTaxCalculation;

namespace PensionCoach.Tools.TaxCalculator.Proprietary;

public class ProprietarySupportedTaxYears : ITaxSupportedYearProvider
{
    public int[] GetSupportedTaxYears()
    {
        int[] years = { 2019 };

        return years;
    }

    public int MapToSupportedYear(int taxYear)
    {
        return GetSupportedTaxYears().Max();
    }
}

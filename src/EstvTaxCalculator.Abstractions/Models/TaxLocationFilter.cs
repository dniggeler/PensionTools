﻿namespace PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

public class TaxLocationRequest
{
    public string Search { get; set; }

    public int Language { get; set; } = 1;
}

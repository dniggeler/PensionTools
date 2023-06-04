﻿using System.Collections.Generic;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace Calculators.CashFlow.Models;

public record CapitalBenefitsTransferInResult
{
    public int StartingYear { get; set; }

    public int NumberOfPeriods { get; set; }

    public IEnumerable<SinglePeriodCalculationResult> DeltaSeries { get; set; }

    public IEnumerable<SinglePeriodCalculationResult> BenchmarkSeries { get; set; }

    public IEnumerable<SinglePeriodCalculationResult> ScenarioSeries { get; set; }
}
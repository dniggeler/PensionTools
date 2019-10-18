﻿using System.Data;
using System.Linq;
using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator
{
    public class TaxPersonValidator : AbstractValidator<TaxPerson>
    {
        private const int MinSupportedYear = 2018;
        private readonly string[] _supportedCantons = {"ZH"};
        public TaxPersonValidator()
        {
            RuleFor(x => x.CalculationYear).Must(x => x >= MinSupportedYear);
            RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
            RuleFor(x => x.DenominationType).Must(x => x.IsSome);
            RuleFor(x => x.Canton).Must(x => _supportedCantons.Contains(x));
        }
    }
}
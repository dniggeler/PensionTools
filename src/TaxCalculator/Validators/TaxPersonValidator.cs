﻿using System.Linq;
using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class TaxPersonValidator : AbstractValidator<TaxPerson>
    {
        public TaxPersonValidator()
        {
            this.RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
            this.RuleFor(x => x.ReligiousGroupType).Must(x => x.IsSome);
            this.RuleFor(x => x.Canton).SetValidator(new CantonValidator());
        }
    }
}
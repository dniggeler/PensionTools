﻿using Application.Tax.Proprietary.Abstractions.Models;
using FluentValidation;

namespace Application.Validators;

/// <inheritdoc />
public class AggregatedTaxResultValidator : AbstractValidator<AggregatedBasisTaxResult>
{
    public AggregatedTaxResultValidator()
    {
        RuleFor(x => x.IncomeTax).NotNull();
        RuleFor(x => x.WealthTax).NotNull();
    }
}
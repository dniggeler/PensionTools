﻿using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using LanguageExt;

namespace Application.Tax.Proprietary.Abstractions;

public interface IFederalCapitalBenefitTaxCalculator
{
    Task<Either<string, BasisTaxResult>> CalculateAsync(
        int calculationYear, FederalTaxPerson person);
}

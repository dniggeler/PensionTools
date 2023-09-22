﻿using Domain.Models.Tax;

namespace Application.Tax.Proprietary.Abstractions.Models.Person;

public record BasisTaxPerson : TaxPersonBasic
{
    public decimal TaxableAmount { get; set; } 
}
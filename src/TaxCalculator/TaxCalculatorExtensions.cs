using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data;

namespace TaxCalculator
{
    public static class TaxCalculatorExtensions
    {
        public static void AddTaxCalculators(this ServiceCollection collection)
        {
            collection.AddTransient<IIncomeTaxCalculator, IncomeTaxCalculator>();
            collection.AddSingleton<IValidator<TaxPerson>, TaxPersonValidator>();
        }
    }
}
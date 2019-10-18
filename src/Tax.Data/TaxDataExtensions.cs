﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tax.Data
{
    public static class TaxDataExtensions
    {
        public static void AddTaxData(this ServiceCollection collection)
        {
            collection.AddDbContext<TaxTariffDbContext>();
            collection.AddDbContext<TaxRateDbContext>();
            collection.AddSingleton<Func<TaxTariffDbContext>>(provider =>
                provider.GetRequiredService<TaxTariffDbContext>);
            collection.AddSingleton<Func<TaxRateDbContext>>(provider =>
                provider.GetRequiredService<TaxRateDbContext>);
        }
    }
}
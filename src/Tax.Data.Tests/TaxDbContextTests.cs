using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Tax.Data.Tests
{
    [Trait("Data", "DB Context")]
    public class TaxDbContextTests : IClassFixture<TaxDataFixture>
    {
        private readonly TaxDataFixture _fixture;
        private readonly ITestOutputHelper _outputHelper;

        public TaxDbContextTests(TaxDataFixture fixture, ITestOutputHelper outputHelper)
        {
            _fixture = fixture;
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "Tax Rate")]
        public void ShouldReturnTaxRateForMunicipality()
        {
            using (var dbContext = _fixture.Provider.GetService<TaxRateDbContext>())
            {
                foreach (var taxItem in dbContext.Rates.Where(item => item.Canton == "ZH" &&
                                                                        item.Year == 2017))
                {
                    _outputHelper.WriteLine($"{taxItem.Municipality}: {taxItem.TaxRateMunicipality}");
                }
            }
        }
    }
}

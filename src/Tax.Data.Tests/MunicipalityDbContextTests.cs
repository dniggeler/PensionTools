using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tax.Data.Abstractions.Models;
using Xunit;

namespace Tax.Data.Tests
{
    [Trait("Data", "Municipality DB Context")]
    public class MunicipalityDbContextTests : IClassFixture<TaxDataFixture>
    {
        private readonly TaxDataFixture _fixture;

        public MunicipalityDbContextTests(TaxDataFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Get Z�rich")]
        public void ShouldLoadSingleMunicipalityByBfsNumber()
        {
            // given
            int bfsNumber = 261;

            // when
            var result = GetMunicipalityEntities();

            // then
            result.Should().NotBeNullOrEmpty();

            IEnumerable<MunicipalityEntity> GetMunicipalityEntities()
            {
                using var dbContext = _fixture.Provider.GetService<MunicipalityDbContext>();
                return dbContext.MunicipalityEntities
                    .Where(item => item.BfsNumber == bfsNumber)
                    .ToList();
            }
        }
    }
}

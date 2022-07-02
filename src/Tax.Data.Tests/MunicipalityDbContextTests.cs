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

        [Fact(DisplayName = "Get Zürich")]
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

        [Fact(DisplayName = "Load Staged Zip Data")]
        public void LoadStagesZipData()
        {
            // given
            int bfsNumber = 261;

            // when
            var result = GetZipEntities();

            // then
            result.Should().NotBeNullOrEmpty();

            IEnumerable<ZipEntity> GetZipEntities()
            {
                using var dbContext = _fixture.Provider.GetService<MunicipalityDbContext>();
                return dbContext.TaxMunicipalityEntities
                    .Where(item => item.BfsNumber == bfsNumber)
                    .ToList();
            }
        }

        [Fact(DisplayName = "Truncate Stage Zip Data Table", Skip = "Preserve date")]
        public void TruncateTaxMunicipalityTable()
        {
            // given

            // when
            using var dbContext = _fixture.Provider.GetService<MunicipalityDbContext>();
            var result = dbContext.TruncateTaxMunicipalityTable();

            // then
            result.Should().BeGreaterThan(0);
        }
    }
}

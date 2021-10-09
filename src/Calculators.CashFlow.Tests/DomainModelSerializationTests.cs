using System;
using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Models;
using Newtonsoft.Json;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests
{
    [Trait("Domain Model", "Serialization")]
    public class DomainModelSerializationTests
    {
        [Fact(DisplayName = "Deserialize Municipality")]
        public void Deserialize_MunicipalityModel()
        {
            // given
            MunicipalityModel model = new MunicipalityModel
            {
                BfsNumber = 261,
                Canton = Canton.ZH,
                Name = "Zürich",
                MutationId = 1911,
                SuccessorId = 33,
                DateOfMutation = new DateTime(2020,1,1)
            };
            string json = JsonConvert.SerializeObject(new[]{ model });

            // when
            var result = JsonConvert.DeserializeObject<IEnumerable<MunicipalityModel>>(json);

            // then
            Snapshot.Match(result);
        }
    }
}

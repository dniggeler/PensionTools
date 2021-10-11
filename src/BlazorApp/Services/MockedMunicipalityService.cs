using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;

namespace BlazorApp.Services
{
    public class MockedMunicipalityService : IMunicipalityService
    {
        public async Task<IEnumerable<MunicipalityModel>> GetAllAsync()
        {
            var municipalities = new List<MunicipalityModel>
            {
                new()
                {
                    BfsNumber = 261,
                    Canton = Canton.ZH,
                    Name = "Zürich",
                },
                new()
                {
                    BfsNumber = 136,
                    Canton = Canton.ZH,
                    Name = "Langnau a.A.",
                },
                new()
                {
                    BfsNumber = 139,
                    Canton = Canton.ZH,
                    Name = "Rüschlikon",
                },
                new()
                {
                    BfsNumber = 3426,
                    Canton = Canton.SG,
                    Name = "Zuzwil (SG)",
                }
            };

            return await Task.FromResult(municipalities);
        }

        public Task<IEnumerable<MunicipalityModel>> GetTaxSupportingAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}

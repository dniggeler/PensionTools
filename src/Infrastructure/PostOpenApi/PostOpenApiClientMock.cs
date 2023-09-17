using System.Globalization;
using Infrastructure.PostOpenApi.Models;
using LanguageExt;

namespace Infrastructure.PostOpenApi;

/// <summary>
/// Not yet used as Post Open API is not part of any user calls.
/// </summary>
public class PostOpenApiClientMock : IPostOpenApiClient
{
    public Task<OpenApiZipInfo> GetZipCodesAsync(int limit, int offset)
    {
        return GetMockData().AsTask();
    }

    private OpenApiZipInfo GetMockData()
    {
        return new OpenApiZipInfo
        {
            TotalCount = 10,
            Records = new List<OpenApiZipRecord>
            {
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 261,
                            Canton = "ZH",
                            LanguageCode = 0,
                            MunicipalityName = "Zürich",
                            ZipCode = "8047",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 951,
                            Canton = "BE",
                            LanguageCode = 0,
                            MunicipalityName = "Affoltern im Emmental",
                            ZipCode = "3416",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 2196,
                            Canton = "FR",
                            LanguageCode = 0,
                            MunicipalityName = "Fribourg",
                            ZipCode = "1700",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 2196,
                            Canton = "AG",
                            LanguageCode = 0,
                            MunicipalityName = "Aarau",
                            ZipCode = "5000",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 6266,
                            Canton = "VS",
                            LanguageCode = 0,
                            MunicipalityName = "Sion",
                            ZipCode = "1950",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 1201,
                            Canton = "UR",
                            LanguageCode = 0,
                            MunicipalityName = "Altdorf UR",
                            ZipCode = "6460",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 1061,
                            Canton = "LU",
                            LanguageCode = 0,
                            MunicipalityName = "Luzern",
                            ZipCode = "6000",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 2762,
                            Canton = "FR",
                            LanguageCode = 0,
                            MunicipalityName = "Allschwil",
                            ZipCode = "4123",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 6742,
                            Canton = "AG",
                            LanguageCode = 0,
                            MunicipalityName = "Les Bois",
                            ZipCode = "2336",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
                new()
                {
                    Record = new OpenApiZipDetail
                    {
                        Fields = new OpenApiZipFields
                        {
                            BfsCode = 3851,
                            Canton = "VS",
                            LanguageCode = 0,
                            MunicipalityName = "Davos Dorf",
                            ZipCode = "7260",
                            ZipCodeAddOn = "00",
                            DateOfValidity = new DateTime(1990, 3, 17).ToString(CultureInfo.InvariantCulture)
                        },
                        TimeStamp = new DateTime(2022, 3, 17)
                    }
                },
            }
        };
    }
}

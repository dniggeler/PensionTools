using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.ViewModels;
using PensionCoach.Tools.CommonTypes;

namespace BlazorApp.Services.Mock;

public class MockedPersonService : IPersonService
{
    private static readonly List<PersonViewModel> Persons;

    static MockedPersonService()
    {
        Persons = GetDefaultPersons();
    }

    public async Task<IEnumerable<PersonViewModel>> GetPersonsAsync()
    {
        return await Task.FromResult(GetDefaultPersons());
    }

    public Task AddPersonAsync(PersonViewModel person)
    {
        Persons.Add(person);

        return Task.CompletedTask;
    }

    public Task DeletePersonAsync(Guid id)
    {
        Persons.RemoveAll(p => p.Id == id);

        return Task.CompletedTask;
    }

    private static List<PersonViewModel> GetDefaultPersons()
    {
        return new List<PersonViewModel>
        {
            new()
            {
                Name = "Tester 1",
                DateOfBirth = new DateTime(1990, 11, 11),
                Gender = Gender.Male,
                CivilStatus = CivilStatus.Single,
                BfsMunicipalityId = 136,
                Canton = Canton.ZH,
                MunicipalityName = "Langnau aA",
                NumberOfChildren = 0,
                ReligiousGroupType = ReligiousGroupType.Protestant,
                TaxableIncome = 100_000,
                TaxableWealth = 500_000,
            },
            new()
            {
                Name = "Tester 2",
                DateOfBirth = new DateTime(1969, 3, 17),
                Gender = Gender.Male,
                CivilStatus = CivilStatus.Married,
                BfsMunicipalityId = 261,
                Canton = Canton.ZH,
                MunicipalityName = "Zürich",
                NumberOfChildren = 0,
                ReligiousGroupType = ReligiousGroupType.Other,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                TaxableIncome = 100_000,
                TaxableWealth = 500_000
            }
        };
    }
}

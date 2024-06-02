using BlazorApp.Services;
using BlazorApp.ViewModels;
using Bunit;
using Domain.Enums;

namespace BlazorApp.Components.Tests;

public class PersonServiceMockTests : TestContext
{
    public PersonServiceMockTests()
    {
        this.AddBlazoredLocalStorage();
        Services.AddMockServices();
    }

    [Fact(DisplayName = "Add New Person")]
    public async Task Add_New_Person()
    {
        // given
        Guid newId = Guid.NewGuid();

        // when
        IPersonService personService = Services.GetService<IPersonService>();

        PersonViewModel result = await AddNewPersonAsync(personService, newId);

        // then
        Assert.True(result is not null);
    }

    [Fact(DisplayName = "Delete Person")]
    public async Task Delete_Person()
    {
        // given
        Guid newId = Guid.NewGuid();

        // when
        IPersonService personService = Services.GetService<IPersonService>();

        await AddNewPersonAsync(personService, newId);

        await personService.DeletePersonAsync(newId);

        var result = await personService.GetAsync(newId);

        // then
        Assert.True(result is null);
    }

    [Fact(DisplayName = "Edit and Save Person")]
    public async Task Edit_And_Save_Person()
    {
        // given
        Guid newId = Guid.NewGuid();
        DateTime birthday = new DateTime(1966, 2, 10);

        // when
        IPersonService personService = Services.GetService<IPersonService>();

        PersonViewModel newPerson = await AddNewPersonAsync(personService, newId);
        newPerson.DateOfBirth = birthday;
        await personService.UpdateAsync(newPerson);

        PersonViewModel result = await personService.GetAsync(newId);

        // then
        Assert.True(result.DateOfBirth == birthday);
    }

    private async Task<PersonViewModel> AddNewPersonAsync(IPersonService personService, Guid id)
    {
        PersonViewModel newPerson = CreatePerson(id);
        await personService.AddAsync(newPerson);

        return await personService.GetAsync(id);
    }

    private PersonViewModel CreatePerson(Guid id)
    {
        return new PersonViewModel
        {
            Id = id,
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
            FinalRetirementPension = 30_000,
            FinalCapital3a = 150_000,
            FinalRetirementCapital = 500_000
        };
    }
}

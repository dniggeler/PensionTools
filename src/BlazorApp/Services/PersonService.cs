using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.ViewModels;
using Blazored.LocalStorage;

namespace BlazorApp.Services;

public class PersonService : IPersonService
{
    private readonly ILocalStorageService localStorageService;

    public PersonService(ILocalStorageService localStorageService)
    {
        this.localStorageService = localStorageService;
    }

    public async Task<IEnumerable<PersonViewModel>> GetPersonsAsync()
    {
        return await GetPersonsFromStorageAsync();
    }

    public async Task AddPersonAsync(PersonViewModel person)
    {
        List<PersonViewModel> persons = (await GetPersonsFromStorageAsync()).ToList();
        persons.Add(person);

        await localStorageService.SetItemAsync<IEnumerable<PersonViewModel>>(nameof(PersonViewModel), persons);
    }

    public async Task DeletePersonAsync(Guid id)
    {
        List<PersonViewModel> persons = (await GetPersonsFromStorageAsync()).ToList();

        persons.RemoveAll(p => p.Id == id);

        await localStorageService.SetItemAsync<IEnumerable<PersonViewModel>>(nameof(PersonViewModel), persons);
    }

    private async Task<IEnumerable<PersonViewModel>> GetPersonsFromStorageAsync()
    {
        var cachedItems = await localStorageService.GetItemAsync<IEnumerable<PersonViewModel>>(nameof(PersonViewModel));

        if (cachedItems is { })
        {
            return cachedItems;
        }

        return Enumerable.Empty<PersonViewModel>();
    }
}

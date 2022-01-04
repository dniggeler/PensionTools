using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.ViewModels;

namespace BlazorApp.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonViewModel>> GetPersonsAsync();

        Task AddPersonAsync(PersonViewModel person);

        Task DeletePersonAsync(Guid id);
    }
}

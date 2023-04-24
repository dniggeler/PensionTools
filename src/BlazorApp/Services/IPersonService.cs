using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.ViewModels;

namespace BlazorApp.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonViewModel>> GetPersonsAsync();

        Task<PersonViewModel> GetAsync(Guid id);

        Task AddAsync(PersonViewModel person);

        Task UpdateAsync(PersonViewModel person);

        Task DeletePersonAsync(Guid id);
    }
}

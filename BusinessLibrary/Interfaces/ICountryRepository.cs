using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLibrary.Interfaces
{
    public interface ICountryRepository
    {
        Task<ActionMessages> Create(Country country);

        Task<CountryWithMessage> Find(Guid id);

        Task<CountryListWithMessage> FindAll();

        Task<CountryWithMessage> Edit(Country country);

        Task<CountryWithMessage> AddCitiesToCountry(AddCitiesToCountryVM country);

        Task<CountryWithMessage> RemoveCitiesFromCountry(RemoveCitiesFromCountryVM country);

        Task<ActionMessages> Delete(Guid id);
    }
}
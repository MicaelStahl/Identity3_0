using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLibrary.Interfaces
{
    public interface ICityRepository
    {
        Task<ActionMessages> Create(City city, Guid? countryId);

        Task<CityWithMessage> Find(Guid id);

        Task<CityListWithMessage> FindAll();

        Task<CityWithMessage> Edit(City city, Guid? countryId); // Redirects to Details

        Task<CityWithMessage> AddPeople(Guid cityId, List<Guid> personId); // Redirects to details

        Task<ActionMessages> Delete(Guid id);
    }
}
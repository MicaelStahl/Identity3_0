using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLibrary.Interfaces
{
    public interface IGlobalRepository
    {
        #region People related

        Task<List<Person>> GetPeople();

        #endregion People related

        #region City related

        Task<Dictionary<Guid, string>> GetCities();

        Task<List<Person>> PeopleNotInCity(City city);

        #endregion City related

        #region Country related

        Task<List<City>> GetCityList(Country country);

        Task<Dictionary<Guid, string>> GetCountries();

        #endregion Country related
    }
}
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLibrary.Interfaces
{
    public interface IGlobalRepository
    {
        Task<List<Person>> GetPeople();

        Task<List<City>> GetCities();

        Task<List<Country>> GetCountries();
    }
}
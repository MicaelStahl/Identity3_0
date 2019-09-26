using Identity3_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity3_0.Interfaces
{
    public interface IGlobalRepository
    {
        Task<List<Person>> GetPeople();

        Task<List<City>> GetCities();

        Task<List<Country>> GetCountries();
    }
}

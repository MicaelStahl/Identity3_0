using BusinessLibrary.Interfaces;
using DataAccessLibrary.Database;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLibrary.Repositories
{
    /// <summary>
    /// Returns a list of respective objects (Cities, Countries, People)
    /// </summary>
    public class GlobalRepository : IGlobalRepository
    {
        #region D.I

        private readonly Identity3_0DbContext _db;
        private readonly DictionaryMessages _dictionary;

        public GlobalRepository(Identity3_0DbContext db)
        {
            _db = db;
            _dictionary = new DictionaryMessages();
        }

        #endregion D.I

        public async Task<Dictionary<Guid, string>> GetCities()
        {
            var cities = new Dictionary<Guid, string>();

            await _db.Cities.ForEachAsync(x => cities.Add(x.Id, x.Name));

            return cities;
        }

        public async Task<Dictionary<Guid, string>> GetCountries()
        {
            var countries = new Dictionary<Guid, string>();

            await _db.Countries.ForEachAsync(x => countries.Add(x.Id, x.Name));

            return countries;
        }

        public async Task<List<Person>> GetPeople()
        {
            return await _db.People.ToListAsync();
        }

        /// <summary>
        /// Returns a list of people not belonging to the requested City.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Person>> PeopleNotInCity(City city)
        {
            return await _db.People.Include(x => x.City).Where(x => x.City != city).ToListAsync();
        }
    }
}
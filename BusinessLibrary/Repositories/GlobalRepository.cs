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
    /// This class is mostly used to not scuff the main repositories with specific methods for specific actions.
    /// </summary>
    public class GlobalRepository : IGlobalRepository
    {
        #region D.I

        private readonly Identity3_0DbContext _db;

        public GlobalRepository(Identity3_0DbContext db)
        {
            _db = db;
        }

        #endregion D.I

        #region People related

        /// <summary>
        /// Returns a list of people.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Person>> GetPeople()
        {
            return await _db.People.ToListAsync();
        }

        #endregion People related

        #region City related

        /// <summary>
        /// Returns a dictionary of cities with Ids' and Names'.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<Guid, string>> GetCities()
        {
            var cities = new Dictionary<Guid, string>();

            await _db.Cities.ForEachAsync(x => cities.Add(x.Id, x.Name));

            return cities;
        }

        /// <summary>
        /// Returns a list of people not belonging to the requested City.
        /// </summary>
        /// <param name="city">The requested city to compare against.</param>
        /// <returns></returns>
        public async Task<List<Person>> PeopleNotInCity(City city)
        {
            return await _db.People.Include(x => x.City).Where(x => x.City != city).ToListAsync();
        }

        #endregion City related

        #region Country related

        /// <summary>
        /// Returns a list of cities not listed in the requested country.
        /// </summary>
        /// <param name="country">Country to compare against</param>
        /// <returns></returns>
        public async Task<List<City>> GetCityList(Country country)
        {
            var cities = await _db.Cities.Include(x => x.Country).Where(x => x.Country.Id != country.Id).ToListAsync();

            if (cities == null || cities.Count == 0)
            {
                throw new NullReferenceException("The requested list was empty.");
            }

            return cities;
        }

        /// <summary>
        /// Returns a dictionary of countries with Ids' and Names'.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<Guid, string>> GetCountries()
        {
            var countries = new Dictionary<Guid, string>();

            await _db.Countries.ForEachAsync(x => countries.Add(x.Id, x.Name));

            return countries;
        }

        #endregion Country related
    }
}
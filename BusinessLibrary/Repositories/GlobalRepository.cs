using Identity3_0.Database;
using Identity3_0.Interfaces;
using Identity3_0.Models;
using Identity3_0.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity3_0.Repositories
{
    public class GlobalRepository : IGlobalRepository
    {
        #region D.I

        private readonly Identity3_0DbContext _db;

        public GlobalRepository(Identity3_0DbContext db)
        {
            _db = db;
        }

        #endregion

        public async Task<List<City>> GetCities()
        {
            return await _db.Cities.Include(x=>x.Country).ToListAsync();
        }

        public async Task<List<Country>> GetCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<List<Person>> GetPeople()
        {
            return await _db.People.Include(x=>x.City).ToListAsync();
        }
    }
}

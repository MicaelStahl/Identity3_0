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
    public class GlobalRepository : IGlobalRepository
    {
        #region D.I

        private readonly Identity3_0DbContext _db;

        public GlobalRepository(Identity3_0DbContext db)
        {
            _db = db;
        }

        #endregion D.I

        public async Task<List<City>> GetCities()
        {
            return await _db.Cities.Include(x => x.Country).ToListAsync();
        }

        public async Task<List<Country>> GetCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<List<Person>> GetPeople()
        {
            return await _db.People.Include(x => x.City).ToListAsync();
        }
    }
}
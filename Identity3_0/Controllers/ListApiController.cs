using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity3_0.Database;
using Identity3_0.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity3_0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListApiController : ControllerBase
    {
        private readonly Identity3_0DbContext _db;

        public ListApiController(Identity3_0DbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns a list of people.
        /// </summary>
        [HttpGet("people")]
        public async Task<List<Person>> GetPeople()
        {
            return await _db.People
                .Include(x=>x.City)
                .ToListAsync();
        }

        /// <summary>
        /// Returns a list of cities.
        /// </summary>
        [HttpGet("cities")]
        public async Task<List<City>> GetCities()
        {
            return await _db.Cities
                .Include(x=>x.People)
                .Include(x=>x.Country)
                .ToListAsync();
        }

        /// <summary>
        /// Returns a list of countries.
        /// </summary>
        [HttpGet("countries")]
        public async Task<List<Country>> GetCountries()
        {
            return await _db.Countries
                .Include(x=>x.Cities)
                .ToListAsync();
        }
    }
}
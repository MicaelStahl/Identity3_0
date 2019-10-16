using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibrary.ViewModels
{
    /// <summary>
    /// General purpose viewmodel that works for most methods.
    /// </summary>
    public class CountryWithMessage
    {
        public Country Country { get; set; }

        public ActionMessages Message { get; set; }
    }

    /// <summary>
    /// General purpose viewmodel containing a list of countries.
    /// </summary>
    public class CountryListWithMessage
    {
        public List<Country> Countries { get; set; }

        public ActionMessages Message { get; set; }
    }

    /// <summary>
    /// The GET for adding cities to country.
    /// </summary>
    public class AddCitiesToCountry
    {
        public KeyValuePair<Guid, string> Country { get; set; }

        public List<City> Cities { get; set; } = new List<City>();
    }

    /// <summary>
    /// The POST for adding cities to country.
    /// </summary>
    public class AddCitiesToCountryVM
    {
        public Guid CountryId { get; set; }

        public List<Guid> CitiesId { get; set; } = new List<Guid>();
    }

    /// <summary>
    /// The GET for removing cities from country.
    /// </summary>
    public class RemoveCitiesFromCountry
    {
        public KeyValuePair<Guid, string> Country { get; set; }

        public List<City> Cities { get; set; } = new List<City>();
    }

    /// <summary>
    /// The POST for removing cities from country.
    /// </summary>
    public class RemoveCitiesFromCountryVM
    {
        public Guid CountryId { get; set; }

        public List<Guid> CitiesId { get; set; } = new List<Guid>();
    }
}
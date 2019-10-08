using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibrary.ViewModels
{
    public class CityWithMessage
    {
        public City City { get; set; }

        public ActionMessages Message { get; set; }
    }

    public class CityListWithMessage
    {
        public List<City> Cities { get; set; }

        public ActionMessages Message { get; set; }
    }

    /// <summary>
    /// Used for creation and contains a list of countries.
    /// </summary>
    public class CityCreation
    {
        public City City { get; set; }

        public ActionMessages Message { get; set; }

        public Dictionary<Guid, string> Countries { get; set; } = new Dictionary<Guid, string>();

        public Guid CountryId { get; set; }
    }

    /// <summary>
    /// The GET for adding people
    /// </summary>
    public class AddPeopleToCity
    {
        public KeyValuePair<Guid, string> City { get; set; }

        public List<Person> People { get; set; } = new List<Person>();
    }

    /// <summary>
    /// The POST for adding people.
    /// </summary>
    public class AddPeopleToCityVM
    {
        public Guid CityId { get; set; }

        public List<Guid> PeopleId { get; set; } = new List<Guid>();
    }

    public class RemovePeopleFromCity
    {
        public Guid CityId { get; set; }

        public List<Guid> PeopleId { get; set; } = new List<Guid>();
    }
}
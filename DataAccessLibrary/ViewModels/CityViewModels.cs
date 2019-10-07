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
    }
}
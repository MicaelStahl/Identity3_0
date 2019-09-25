using Identity3_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity3_0.ViewModels
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
    public class CityCreation : CountryListWithMessage
    {
    }
}

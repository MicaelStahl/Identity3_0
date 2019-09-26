using Identity3_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity3_0.ViewModels
{
    public class CountryWithMessage
    {
        public Country Country { get; set; }

        public ActionMessages Message { get; set; }
    }

    public class CountryListWithMessage
    {
        public List<Country> Countries { get; set; }

        public ActionMessages Message { get; set; }
    }
}

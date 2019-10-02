using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibrary.ViewModels
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
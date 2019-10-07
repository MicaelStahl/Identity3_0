using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibrary.ViewModels
{
    public class PersonWithMessage
    {
        public Person Person { get; set; }

        public ActionMessages Message { get; set; }
    }

    public class PersonListWithMessage
    {
        public List<Person> People { get; set; }

        public ActionMessages Message { get; set; }
    }

    /// <summary>
    /// Used for creation and contains a list of cities.
    /// </summary>
    public class PersonUpdate
    {
        public Person Person { get; set; }

        public Guid CityId { get; set; }

        public ActionMessages Message { get; set; }

        public Dictionary<Guid, string> Cities { get; set; } = new Dictionary<Guid, string>();
    }

    public class PersonCreate
    {
        public Person Person { get; set; }

        public Guid CityId { get; set; }
    }
}
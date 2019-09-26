using Identity3_0.Models;
using Identity3_0.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity3_0.Interfaces
{
    public interface IPersonRepository
    {
        Task<ActionMessages> Create(Person person, Guid cityId);

        Task<PersonWithMessage> Find(Guid id);

        Task<PersonListWithMessage> FindAll();

        Task<ActionMessages> Edit(Person person);

        Task<ActionMessages> Delete(Guid id);
    }
}
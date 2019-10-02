using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLibrary.Interfaces
{
    public interface IPersonRepository
    {
        Task<ActionMessages> Create(Person person, Guid? cityId);

        Task<PersonWithMessage> Find(Guid id);

        Task<PersonListWithMessage> FindAll();

        Task<ActionMessages> Edit(Person person, Guid? cityId);

        Task<ActionMessages> Delete(Guid id);
    }
}
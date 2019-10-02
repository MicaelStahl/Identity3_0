using BusinessLibrary.Interfaces;
using DataAccessLibrary.Database;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BusinessLibrary.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        #region D.I

        private readonly Identity3_0DbContext _db;

        public PersonRepository(Identity3_0DbContext db)
        {
            _db = db;
        }

        #endregion D.I

        #region Create

        public async Task<ActionMessages> Create(Person person, Guid? cityId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(person.FirstName) || string.IsNullOrWhiteSpace(person.LastName) ||
                    string.IsNullOrWhiteSpace(person.Email) || string.IsNullOrWhiteSpace(person.PhoneNumber))
                {
                    return ActionMessages.FillAllFields;
                }

                if (person.Age > 110 || person.Age < 1)
                {
                    return ActionMessages.InvalidAge;
                }

                await _db.People.AddAsync(new Person { FirstName = person.FirstName, LastName = person.LastName, Age = person.Age, Email = person.Email, PhoneNumber = person.PhoneNumber, City = await _db.Cities.SingleOrDefaultAsync(x => x.Id == cityId) ?? null });

                await _db.SaveChangesAsync();

                return ActionMessages.Created;
            }
            catch // Returns Failed if an exception were to be thrown.
            {
                return ActionMessages.Failed;
            }
        }

        #endregion Create

        #region Find

        public async Task<PersonWithMessage> Find(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception();
                }

                var person = await _db.People
                    .Include(x => x.City)
                    .SingleOrDefaultAsync(x => x.Id == id);

                if (person == null)
                {
                    return new PersonWithMessage { Message = ActionMessages.NotFound };
                }

                return new PersonWithMessage { Message = ActionMessages.Success, Person = person };
            }
            catch
            {
                return new PersonWithMessage { Message = ActionMessages.Failed };
            }
        }

        public async Task<PersonListWithMessage> FindAll()
        {
            try
            {
                var people = await _db.People
                    .Include(x => x.City)
                    .ToListAsync();

                if (people == null || people.Count == 0)
                {
                    return new PersonListWithMessage { Message = ActionMessages.Empty };
                }

                return new PersonListWithMessage { Message = ActionMessages.Success, People = people };
            }
            catch
            {
                return new PersonListWithMessage { Message = ActionMessages.Failed };
            }
        }

        #endregion Find

        #region Edit

        public async Task<ActionMessages> Edit(Person person, Guid? cityId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(person.FirstName) || string.IsNullOrWhiteSpace(person.LastName) ||
                    string.IsNullOrWhiteSpace(person.Email) || string.IsNullOrWhiteSpace(person.PhoneNumber))
                {
                    return ActionMessages.FillAllFields;
                }

                if (person.Age > 110 || person.Age < 1)
                {
                    return ActionMessages.InvalidAge;
                }

                person.City = await _db.Cities.SingleOrDefaultAsync(x => x.Id == cityId);

                var original = await _db.People.Include(x => x.City).SingleOrDefaultAsync(x => x.Id == person.Id);

                if (original == null)
                {
                    return ActionMessages.NotFound;
                }

                original.FirstName = person.FirstName;
                original.LastName = person.LastName;
                original.Age = person.Age;
                original.Email = person.Email;
                original.PhoneNumber = person.PhoneNumber;
                original.City = person.City;

                await _db.SaveChangesAsync();

                return ActionMessages.Updated;
            }
            catch
            {
                return ActionMessages.Failed;
            }
        }

        #endregion Edit

        #region Delete

        public async Task<ActionMessages> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception();
                }

                var person = await _db.People.SingleOrDefaultAsync(x => x.Id == id);

                if (person == null)
                {
                    return ActionMessages.NotFound;
                }

                _db.People.Remove(person);

                await _db.SaveChangesAsync();

                return ActionMessages.Deleted;
            }
            catch
            {
                return ActionMessages.Failed;
            }
        }

        #endregion Delete
    }
}
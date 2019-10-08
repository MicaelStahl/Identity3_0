using BusinessLibrary.Interfaces;
using DataAccessLibrary.Database;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLibrary.Repositories
{
    public class CityRepository : ICityRepository
    {
        #region D.I

        private readonly Identity3_0DbContext _db;

        public CityRepository(Identity3_0DbContext db)
        {
            _db = db;
        }

        #endregion D.I

        #region Create

        public async Task<ActionMessages> Create(City city, Guid? countryId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city.Name) || string.IsNullOrWhiteSpace(city.Population) ||
                    string.IsNullOrWhiteSpace(city.PostalCode))
                {
                    return ActionMessages.FillAllFields;
                }

                await _db.Cities.AddAsync(new City { Name = city.Name, PostalCode = city.PostalCode, Population = city.Population, Country = await _db.Countries.SingleOrDefaultAsync(x => x.Id == countryId) ?? null });

                await _db.SaveChangesAsync();

                return ActionMessages.Created;
            }
            catch (Exception)
            {
                return ActionMessages.Failed;
            }
        }

        #endregion Create

        #region Find

        public async Task<CityWithMessage> Find(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception();
                }

                var city = await _db.Cities
                    .Include(x => x.Country)
                    .Include(x => x.People)
                    .SingleOrDefaultAsync(x => x.Id == id);

                if (city == null)
                {
                    return new CityWithMessage { Message = ActionMessages.NotFound };
                }

                return new CityWithMessage { Message = ActionMessages.Success, City = city };
            }
            catch (Exception)
            {
                return new CityWithMessage { Message = ActionMessages.Failed };
            }
        }

        public async Task<CityListWithMessage> FindAll()
        {
            try
            {
                var cities = await _db.Cities
                    .Include(x => x.Country)
                    .ToListAsync();

                if (cities == null || cities.Count == 0)
                {
                    return new CityListWithMessage { Message = ActionMessages.Empty };
                }

                return new CityListWithMessage { Message = ActionMessages.Success, Cities = cities };
            }
            catch (Exception)
            {
                return new CityListWithMessage { Message = ActionMessages.Failed };
                throw;
            }
        }

        #endregion Find

        #region Edit

        public async Task<CityWithMessage> Edit(City city, Guid? countryId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city.Name) || string.IsNullOrWhiteSpace(city.Population) ||
                    string.IsNullOrWhiteSpace(city.PostalCode))
                {
                    return new CityWithMessage { Message = ActionMessages.FillAllFields };
                }

                var original = await _db.Cities.SingleOrDefaultAsync(x => x.Id == city.Id);

                if (original == null)
                {
                    return new CityWithMessage { Message = ActionMessages.NotFound };
                }
                var country = await _db.Countries.SingleOrDefaultAsync(x => x.Id == countryId) ?? null;

                original.Name = city.Name;
                original.Population = city.Population;
                original.PostalCode = city.PostalCode;
                original.Country = country;

                await _db.SaveChangesAsync();

                return new CityWithMessage { City = original, Message = ActionMessages.Updated };
            }
            catch (Exception)
            {
                return new CityWithMessage { Message = ActionMessages.Failed };
            }
        }

        #region AddPeople

        public async Task<CityWithMessage> AddPeople(Guid cityId, List<Guid> personId)
        {
            try
            {
                if (cityId == Guid.Empty)
                {
                    throw new Exception();
                }

                if (personId == null || personId.Count == 0)
                {
                    throw new Exception();
                }

                var city = await _db.Cities
                    .Include(x => x.People)
                    .Include(x => x.Country)
                    .SingleOrDefaultAsync(x => x.Id == cityId);

                if (city == null)
                {
                    return new CityWithMessage { Message = ActionMessages.NotFound };
                }

                // adds a person to the city for each id in the list.
                foreach (var id in personId)
                {
                    var person = await _db.People.SingleOrDefaultAsync(x => x.Id == id) ?? throw new Exception();
                    if (!city.People.Contains(person))
                    {
                        city.People.Add(person);
                    }
                }
                //personId.ForEach(
                //    async x =>
                //        city.People.Add(
                //            !city.People.Contains( // Stops duplicates from being added.
                //                await _db.People.SingleOrDefaultAsync(c => c.Id == x)) ?
                //                    await _db.People.SingleOrDefaultAsync(c => c.Id == x) : null)); // Else do nothing.

                await _db.SaveChangesAsync();

                return new CityWithMessage { City = city, Message = ActionMessages.Updated };
            }
            catch (Exception)
            {
                return new CityWithMessage { Message = ActionMessages.Failed };
            }
        }

        #endregion AddPeople

        #region RemovePeople

        public async Task<CityWithMessage> RemovePeople(RemovePeopleFromCity remove)
        {
            try
            {
                if (remove.CityId == Guid.Empty)
                {
                    throw new Exception();
                }

                if (remove.PeopleId == null || remove.PeopleId.Count == 0)
                {
                    return new CityWithMessage { Message = ActionMessages.Empty };
                }

                var city = await _db.Cities
                    .Include(x => x.People) // IMPORTANT: Forget this and it won't remove any people.
                    .Include(x => x.Country) // And this for the View.
                    .SingleOrDefaultAsync(x => x.Id == remove.CityId);

                if (city == null)
                {
                    return new CityWithMessage { Message = ActionMessages.NotFound };
                }

                foreach (var personId in remove.PeopleId)
                {
                    var person = await _db.People.SingleOrDefaultAsync(x => x.Id == personId) ?? throw new Exception();

                    if (city.People.Contains(person))
                    {
                        city.People.Remove(person);
                    }
                }
                // Removes the correct person for every Id in the personId list.
                remove.PeopleId.ForEach(
                    async x =>
                        city.People.Remove(
                            city.People.Contains( // Checks that the person actually exists in the city.
                                await _db.People.SingleOrDefaultAsync(c => c.Id == x)) ?
                                    await _db.People.SingleOrDefaultAsync(c => c.Id == x) : null)); // Else do nothing.

                await _db.SaveChangesAsync();

                return new CityWithMessage { City = city, Message = ActionMessages.Success };
            }
            catch (Exception)
            {
                return new CityWithMessage { Message = ActionMessages.Failed };
            }
        }

        #endregion RemovePeople

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

                var city = await _db.Cities.SingleOrDefaultAsync(x => x.Id == id);

                if (city == null)
                {
                    return ActionMessages.NotFound;
                }

                _db.Cities.Remove(city);

                await _db.SaveChangesAsync();

                return ActionMessages.Deleted;
            }
            catch (Exception)
            {
                return ActionMessages.Failed;
            }
        }

        #endregion Delete
    }
}
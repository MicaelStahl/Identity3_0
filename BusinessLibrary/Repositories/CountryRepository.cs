using BusinessLibrary.Interfaces;
using DataAccessLibrary.Database;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLibrary.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        #region D.I

        private readonly Identity3_0DbContext _db;

        public CountryRepository(Identity3_0DbContext db)
        {
            _db = db;
        }

        #endregion D.I

        #region Create

        public async Task<ActionMessages> Create(Country country)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(country.Name) || string.IsNullOrWhiteSpace(country.Population))
                {
                    return ActionMessages.FillAllFields;
                }

                await _db.Countries.AddAsync(new Country { Name = country.Name, Population = country.Population });

                await _db.SaveChangesAsync();

                return ActionMessages.Created;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        #endregion Create

        #region Find

        public async Task<CountryWithMessage> Find(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception("Unexpected error occurred: the Id received was blank.");
                }

                var country = await _db.Countries.Include(x => x.Cities).FirstOrDefaultAsync(x => x.Id == id);

                if (country == null)
                {
                    return new CountryWithMessage { Message = ActionMessages.NotFound };
                }

                return new CountryWithMessage { Country = country, Message = ActionMessages.Success };
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
        }

        public async Task<CountryListWithMessage> FindAll()
        {
            try
            {
                var countries = await _db.Countries.ToListAsync();

                if (countries == null || countries.Count == 0)
                {
                    throw new Exception("Unexpected error occurred: No countries were found.");
                }

                return new CountryListWithMessage { Countries = countries, Message = ActionMessages.Success };
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
        }

        #endregion Find

        #region Edit - Section

        #region Edit

        public async Task<CountryWithMessage> Edit(Country country)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(country.Name) || string.IsNullOrWhiteSpace(country.Population))
                {
                    return new CountryWithMessage { Message = ActionMessages.FillAllFields };
                }

                var original = await _db.Countries.FirstOrDefaultAsync(x => x.Id == country.Id);

                if (original == null)
                {
                    return new CountryWithMessage { Message = ActionMessages.NotFound };
                }

                original.Name = country.Name;
                original.Population = country.Population;

                await _db.SaveChangesAsync();

                return new CountryWithMessage { Country = original, Message = ActionMessages.Updated };
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
        }

        #endregion Edit

        #region AddCities

        public async Task<CountryWithMessage> AddCitiesToCountry(AddCitiesToCountryVM country)
        {
            try
            {
                if (country.CountryId == Guid.Empty)
                {
                    throw new Exception("Unexpected error occurred: The countryId was blank.");
                }

                if (country.CitiesId == null || country.CitiesId.Count == 0)
                {
                    throw new ArgumentNullException("CitiesId", "Unexpected error occurred: the list was null.");
                }

                var original = await _db.Countries.Include(x => x.Cities).FirstOrDefaultAsync(x => x.Id == country.CountryId);

                if (original == null)
                {
                    throw new ArgumentNullException("original", "Unexpected error occurred: country was null.");
                }

                foreach (var cityId in country.CitiesId)
                {
                    var city = await _db.Cities.FirstOrDefaultAsync(x => x.Id == cityId);

                    if (city == null)
                    {
                        throw new Exception($"Unexpected error occurred: No city was found with the given ID: {cityId}");
                    }
                    else if (original.Cities.Contains(city))
                    {
                        throw new ArgumentException($"Unexpected error occurred: {city.Name} already exists in {original.Name}.", "original");
                    }

                    original.Cities.Add(city);
                }

                await _db.SaveChangesAsync();

                return new CountryWithMessage { Country = original, Message = ActionMessages.Updated };
            }
            catch (ArgumentException ex)
            {
                throw new DuplicateWaitObjectException(ex.ParamName, ex.Message);
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
        }

        #endregion AddCities

        #region RemoveCities

        public async Task<CountryWithMessage> RemoveCitiesFromCountry(RemoveCitiesFromCountryVM country)
        {
            try
            {
                if (country.CountryId == Guid.Empty)
                {
                    throw new Exception("Unexpected error occurred: CountryId was blank.");
                }

                if (country.CitiesId == null || country.CitiesId.Count == 0)
                {
                    throw new Exception("Unexpected error occurred: No ID's were found in list.");
                }

                var original = await _db.Countries.Include(x => x.Cities).FirstOrDefaultAsync(x => x.Id == country.CountryId);

                if (original == null)
                {
                    return new CountryWithMessage { Message = ActionMessages.NotFound };
                }

                foreach (var cityId in country.CitiesId)
                {
                    var city = await _db.Cities.FirstOrDefaultAsync(x => x.Id == cityId);

                    if (original.Cities.Contains(city))
                    {
                        original.Cities.Remove(city);
                    }
                }

                await _db.SaveChangesAsync();

                return new CountryWithMessage { Country = original, Message = ActionMessages.Updated };
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
        }

        #endregion RemoveCities

        #endregion Edit - Section

        #region Delete

        public async Task<ActionMessages> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new NullReferenceException("Unexpected error occurred: The ID was blank.");
                }

                var country = await _db.Countries.FirstOrDefaultAsync(x => x.Id == id);

                if (country == null)
                {
                    throw new NullReferenceException($"Unexpected error occurred: Could not find a country with the given ID: {id}");
                }

                _db.Countries.Remove(country);

                await _db.SaveChangesAsync();

                return ActionMessages.Deleted;
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
        }

        #endregion Delete
    }
}
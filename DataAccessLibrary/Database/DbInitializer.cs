using System;
using System.Linq;
using Identity3_0.Database;
using Identity3_0.Models;
using Identity3_0.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Identity3_0
{
    public class DbInitializer
    {
        public static void Initializer(Identity3_0DbContext context, UserManager<AppUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            context.Database.EnsureCreated();

            #region Role Creation

            if (!_roleManager.RoleExistsAsync("Administrator").Result)
            {
                var role = new IdentityRole("Administrator");

                _roleManager.CreateAsync(role).Wait();
            }

            if (!_roleManager.RoleExistsAsync("NormalUser").Result)
            {
                var role = new IdentityRole("NormalUser");

                _roleManager.CreateAsync(role).Wait();
            }

            #endregion

            #region User Creation

            if (_userManager.FindByNameAsync("Administrator").Result == null)
            {
                var user = new AppUser
                {
                    UserName = "Admin.Administrator@context.com",
                    FirstName = "Admin",
                    LastName = "Administrator",
                    Age = 29,
                    Email = "Admin.Administrator@context.com", // Used for signing in.
                    PhoneNumber = "123456789",
                    IsAdmin = true
                };

                var result = _userManager.CreateAsync(user, "Password!23").Result;

                if (result.Succeeded)
                {
                    string[] roles = new string[] { "Administrator", "NormalUser" };

                    _userManager.AddToRolesAsync(user, roles).Wait();
                }
            }

            if (_userManager.FindByNameAsync("NormalUser").Result == null)
            {
                var user = new AppUser
                {
                    UserName = "NormalUser@context.com",
                    FirstName = "Normal",
                    LastName = "User",
                    Age = 20,
                    Email = "NormalUser@context.com", // Used for signing in.
                    PhoneNumber = "123456789",
                    IsAdmin = false
                };

                var result = _userManager.CreateAsync(user, "Password!23").Result;

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "NormalUser").Wait();
                }
            }

            #endregion

            #region Person Creation

            if (context.People.Any())
            {
                return;
            }

            var people = new Person[]
            {
                new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "1", Age = DateTime.Now.Year - 1996, Email = "Test1@context.com", PhoneNumber = "123456789" },
                new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "2", Age = DateTime.Now.Year - 1972, Email = "Test2@context.com", PhoneNumber = "123456789" },
                new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "3", Age = DateTime.Now.Year - 1981, Email = "Test3@context.com", PhoneNumber = "123456789" },
                new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "4", Age = DateTime.Now.Year - 2004, Email = "Test4@context.com", PhoneNumber = "123456789" },
                new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "5", Age = DateTime.Now.Year - 2017, Email = "Test5@context.com", PhoneNumber = "123456789" },
                new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "6", Age = DateTime.Now.Year - 1953, Email = "Test6@context.com", PhoneNumber = "123456789" },
                new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "7", Age = DateTime.Now.Year - 2017, Email = "Test5@context.com", PhoneNumber = "123456789" },
                new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "8", Age = DateTime.Now.Year - 2017, Email = "Test5@context.com", PhoneNumber = "123456789" },

            };

            context.People.AddRange(people);

            context.SaveChanges();

            #endregion

            #region City Creation

            if (context.Cities.Any())
            {
                return;
            }

            var cities = new City[]
            {
                new City { Id = Guid.NewGuid(), Name = "City1", Population = "123456", PostalCode = "57401-57450", People = new Person[] { people[0], people[1] }.ToList() },
                new City { Id = Guid.NewGuid(), Name = "City2", Population = "123", PostalCode = "57451-57500", People = new Person[] { people[2], people[3] }.ToList() },
                new City { Id = Guid.NewGuid(), Name = "City3", Population = "123789", PostalCode = "57501-57550", People = new Person[] { people[4], people[5] }.ToList() },
                new City { Id = Guid.NewGuid(), Name = "City4", Population = "78945", PostalCode = "57551-57600", People = new Person[] { people[6], people[7] }.ToList() }
            };

            context.Cities.AddRange(cities);

            context.SaveChanges();

            #endregion

            #region Country Creation

            var countries = new Country[]
            {
                new Country { Id = Guid.NewGuid(), Name = "Country1", Population = "1239875" , Cities = new City[] { cities[0], cities[1] }.ToList() },
                new Country { Id = Guid.NewGuid(), Name = "Country1", Population = "1239875" , Cities = new City[] { cities[2], cities[3] }.ToList() },

            };

            context.Countries.AddRange(countries);

            context.SaveChanges();

            #endregion
        }
    }
}
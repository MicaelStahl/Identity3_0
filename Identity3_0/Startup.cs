using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Database;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using BusinessLibrary.Interfaces;
using BusinessLibrary.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using DataAccessLibrary.Models;

namespace Identity3_0
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Creates the database.
            services.AddDbContext<Identity3_0DbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            // Scoped values, making dependency injection possible.
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IGlobalRepository, GlobalRepository>();
            services.AddScoped<IAccountValidation, AccountValidation>();
            services.AddSingleton<IEmailSenderUpdated, EmailSender>();

            // A tiny configuration allowing access to "EmailSettings" in appsettings.json.
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            // Adds identity to the application with unique emails.
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<Identity3_0DbContext>()
                .AddDefaultTokenProviders();

            // Makes it possible for frameworks to add this into their dependencies.
            services.AddDistributedMemoryCache();
            // Creates a session on the application that lasts for 20 minutes.
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });

            // Cookie configuration.
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Further cookie configurations.
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.None;

                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Identity options that specifies length on password, lockout, unique email etc.
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;

                options.Lockout.AllowedForNewUsers = true; // Default
                options.Lockout.MaxFailedAccessAttempts = 5; // Default
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Default

                options.User.AllowedUserNameCharacters += "Â‰ˆ≈ƒ÷";
                options.User.RequireUniqueEmail = true;
            });

            // Hashes the password 200_000 times for security.
            services.Configure<PasswordHasherOptions>(options =>
            {
                options.IterationCount = 200_000;
            });

            // Adds Mvc services, a new JSonStringEnumConverter and sets the compatibility version.
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            // Adds several specific paths.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "Account",
                    pattern: "{controller=Account}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "Person",
                    pattern: "World/{controller=Person}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "City",
                    pattern: "World/{controller=City}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "Country",
                    pattern: "World/{controller=Country}/{action=Index}/{id?}");
            });
        }
    }
}
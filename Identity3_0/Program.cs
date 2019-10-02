using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Database;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity3_0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host);

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                //
                // Summary:
                // For readability;
                //  var context = services.GetRequiredService<Identity3_0DbContext>();
                //  var _userManager = services.GetRequiredService<UserManager<AppUser>>();
                //  var _roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                //  Identity3_0Initializer.Initializer(context, _userManager, _roleManager);

                DbInitializer.Initializer(services.GetRequiredService<Identity3_0DbContext>(),
                    services.GetRequiredService<UserManager<AppUser>>(), services.GetRequiredService<RoleManager<IdentityRole>>());
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured while seeding the database.");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
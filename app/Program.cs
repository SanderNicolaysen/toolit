using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using app.Data;
using app.Models;

namespace app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var env = services.GetRequiredService<IHostingEnvironment>();
                var db = services.GetRequiredService<ApplicationDbContext>();
                var um = services.GetRequiredService<UserManager<ApplicationUser>>();
                var rm = services.GetRequiredService<RoleManager<IdentityRole>>();
                
                ApplicationDbInitializer.Initialize(db, um, rm, env.IsDevelopment());
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}

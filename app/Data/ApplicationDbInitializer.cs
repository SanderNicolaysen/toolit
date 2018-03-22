using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

using app.Data;
using app.Models;

namespace app.Data
{
    public static class ApplicationDbInitializer
    {
        public static void Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm, bool isDevelopment)
        {
            if (!isDevelopment)
            {
                db.Database.EnsureCreated();
                return;
            }

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Add roles
            var adminRole = new IdentityRole("Admin");
            rm.CreateAsync(adminRole).Wait();

            // Add users
            var admin = new ApplicationUser { UserName = "admin@uia.no", Email = "admin@uia.no", ChangePassword = true };
            um.CreateAsync(admin, "Password1.").Wait();
            um.AddToRoleAsync(admin, "Admin");

            var user1 = new ApplicationUser { UserName = "ole@uia.no", Email = "ole@uia.no", ChangePassword = true };
            um.CreateAsync(user1, "Password1.");

            var user2 = new ApplicationUser { UserName = "dole@uia.no", Email = "dole@uia.no", ChangePassword = true };
            um.CreateAsync(user2, "Password1.");

            var user3 = new ApplicationUser { UserName = "doffen@uia.no", Email = "doffen@uia.no", ChangePassword = true };
            um.CreateAsync(user3, "Password1.");

            // Add dummy data here
            var tools = new List<Tool>
            {
                new Tool("Skrujern", "available"),
                new Tool("Hammer", "available"),
                new Tool("Sag", "available")
            };

            db.AddRange(tools);

            db.SaveChanges();
        }
    }
}
using Microsoft.AspNetCore.Identity;
using System;
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
                new Tool("Skrujern", "Available", new List<Report>(new Report[] { new Report("It misses a handle!", admin.Id), new Report("Should be replaced.", user1.Id), new Report("It doesn't work", user2.Id) }), new List<Alarm>(new Alarm[] { new Alarm("Sertifisering", new DateTime(2018,4,11)), new Alarm("Årskontroll", new DateTime(2018,4,15))})),
                new Tool("Hammer", "Available", new List<Report>() { new Report("How does it work?", user3.Id) }, new List<Alarm>(new Alarm[] { new Alarm("Sertifisering", new DateTime(2018,4,13))})),
                new Tool("Sag", "Available", new List<Report>(), new List<Alarm>()),
                new Tool("Vater", "Not available", new List<Report>(new Report[] { new Report("Random report.", admin.Id), new Report("What is this thing?", user2.Id) }), new List<Alarm>(new Alarm[] { new Alarm("Årskontroll", new DateTime(2018,4,12))})),
                new Tool("Kniv", "Available", new List<Report>() { new Report("Test", user1.Id) }, new List<Alarm>())
            };

            foreach (var tool in tools)
            {
                tool.Image = "images\\example_tools\\" + tool.Name.ToLower() + ".jpg";
                tool.Thumbnail = "images\\example_tools\\" + tool.Name.ToLower() + "_thumb.jpg";
            }

            db.AddRange(tools);

            db.SaveChanges();
        }
    }
}
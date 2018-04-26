using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

using app.Data;
using app.Models;
using app.Services;

namespace app.Data
{
    public static class ApplicationDbInitializer
    {
        public static void Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm, bool isDevelopment, INotificationManager nm)
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

            var statusAvailable = new Status { StatusName = "Available", Style = "color: #8EE783", Glyphicon = "glyphicon glyphicon-ok-sign" };
            db.Add(statusAvailable);

            var statusBusy = new Status { StatusName = "Busy", Style = "color: #FA7D12", Glyphicon = "glyphicon glyphicon-remove-sign" };
            db.Add(statusBusy);

            var statusBooked = new Status { StatusName = "Booked", Style = "color: #F6D846", Glyphicon = "glyphicon glyphicon-minus-sign" };
            db.Add(statusBooked);

            // Add dummy data here
            var tools = new List<Tool>
            {
                new Tool("Skrujern", statusAvailable, new List<Report>(new Report[] { new Report("It misses a handle!", admin.Id), new Report("Should be replaced.", user1.Id), new Report("It doesn't work", user2.Id) }), new List<Alarm>(new Alarm[] { new Alarm("Sertifisering", new DateTime(2018,4,11)), new Alarm("Årskontroll", new DateTime(2018,4,15))}), "playboy"),
                new Tool("Hammer", statusAvailable, new List<Report>() { new Report("How does it work?", user3.Id) }, new List<Alarm>(new Alarm[] { new Alarm("Sertifisering", new DateTime(2018,4,13))}), "banger"),
                new Tool("Sag", statusBooked, new List<Report>(), new List<Alarm>(), "cutter"),
                new Tool("Vater", statusBusy, new List<Report>(new Report[] { new Report("Random report.", admin.Id), new Report("What is this thing?", user2.Id) }), new List<Alarm>(new Alarm[] { new Alarm("Årskontroll", new DateTime(2018,4,12))}), "måler"),
                new Tool("Kniv", statusAvailable, new List<Report>() { new Report("Test", user1.Id) }, new List<Alarm>(), "stikker")
            };

            foreach (var tool in tools)
            {
                tool.Image = "images/example_tools/" + tool.Name.ToLower() + ".jpg";
                tool.Thumbnail = "images/example_tools/" + tool.Name.ToLower() + "_thumb.jpg";
            }

            db.AddRange(tools);
            db.SaveChanges();

            var logs = new List<Log>
            {
                new Log(1, admin.Id, new DateTime(2018,4,26).ToString(), new DateTime(2018,4,27).ToString()),
                new Log(1, admin.Id, new DateTime(2018,4,28).ToString(), new DateTime(2018,4,29).ToString())
            };


            // Adding some dummy notifications
            nm.SendNotificationAsync(admin.Id, "Verktøyet 'Hammer' har forsvunnet!", "/Tool/Details/2").Wait();
            nm.SendNotificationAsync(admin.Id, "Noen har glemt å levere tilbake 'Vater' etter reservasjonen utgitt!", "/Tool/Details/4").Wait();

            db.AddRange(logs);
            db.SaveChanges();
        }
    }
}
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

            var superAdminRole = new IdentityRole("SuperAdmin");
            rm.CreateAsync(superAdminRole).Wait();

            // Add users
            var admin = new ApplicationUser { UserName = "admin@uia.no", Email = "admin@uia.no", ChangePassword = true, isAdmin = true, isSuperAdmin = true };
            um.CreateAsync(admin, "Password1.").Wait();
            um.AddToRolesAsync(admin, new List<string>(new string[]{"Admin", "SuperAdmin"}));

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
                new Tool("Skrujern", statusAvailable, new List<Report>(new Report[] { new Report(1, "It misses a handle!", admin.Id), new Report(1, "Should be replaced.", user1.Id), new Report(1, "It doesn't work", user2.Id) }), new List<Alarm>(new Alarm[] { new Alarm("Sertifisering", new DateTime(2018,4,11)), new Alarm("Årskontroll", new DateTime(2018,4,15))}), "playboy"),
                new Tool("Hammer", statusAvailable, new List<Report>() { new Report(2, "How does it work?", user3.Id) }, new List<Alarm>(new Alarm[] { new Alarm("Sertifisering", new DateTime(2018,4,13))}), "banger"),
                new Tool("Sag", statusBooked, new List<Report>(), new List<Alarm>(), "cutter"),
                new Tool("Vater", statusBusy, new List<Report>(new Report[] { new Report(4, "Random report.", admin.Id), new Report(4, "What is this thing?", user2.Id) }), new List<Alarm>(new Alarm[] { new Alarm("Årskontroll", new DateTime(2018,4,12))}), "måler"),
                new Tool("Kniv", statusAvailable, new List<Report>() { new Report(5, "Test", user1.Id) }, new List<Alarm>(), "stikker")
            };

            foreach (var tool in tools)
            {
                tool.Image = "images/example_tools/" + tool.Name.ToLower() + ".jpg";
                tool.Thumbnail = "images/example_tools/" + tool.Name.ToLower() + "_thumb.jpg";
            }

            db.AddRange(tools);

            // Adding some dummy notifications
            nm.SendNotificationAsync(admin.Id, "Verktøyet 'Hammer' har forsvunnet!", "/Tool/Details/2").Wait();
            nm.SendNotificationAsync(admin.Id, "Noen har glemt å levere tilbake 'Vater' etter reservasjonen utgitt!", "/Tool/Details/4").Wait();

            db.SaveChanges();

            var now = DateTime.Now;
            var logs = new List<Log>
            {
                new Log(tools[0].Id, user1.Id, now.Subtract(TimeSpan.FromDays(124)), now.Subtract(TimeSpan.FromDays(118))),
                new Log(tools[0].Id, user2.Id, now.Subtract(TimeSpan.FromDays(60)), now.Subtract(TimeSpan.FromDays(20))),
                new Log(tools[0].Id, user3.Id, now.Subtract(TimeSpan.FromDays(18)), now.Subtract(TimeSpan.FromDays(5))),
                new Log(tools[0].Id, user3.Id, now.Subtract(TimeSpan.FromDays(5)), now.Subtract(TimeSpan.FromDays(4))),
                new Log(tools[0].Id, user1.Id, now.Subtract(TimeSpan.FromDays(3)), now.Subtract(TimeSpan.FromDays(3))),
                new Log(tools[1].Id, user3.Id, now.Subtract(TimeSpan.FromDays(365)), now.Subtract(TimeSpan.FromDays(150))),
                new Log(tools[1].Id, user1.Id, now.Subtract(TimeSpan.FromDays(2)), now.Subtract(TimeSpan.FromDays(1))),
                new Log(tools[2].Id, user2.Id, now.Subtract(TimeSpan.FromDays(2)), now.Subtract(TimeSpan.FromDays(2))),
                new Log(tools[3].Id, user2.Id, now.Subtract(TimeSpan.FromDays(450)), now.Subtract(TimeSpan.FromDays(258))),
                new Log(tools[3].Id, user3.Id, now.Subtract(TimeSpan.FromDays(226)), now.Subtract(TimeSpan.FromDays(204))),
                new Log(tools[3].Id, user1.Id, now.Subtract(TimeSpan.FromDays(170)), now.Subtract(TimeSpan.FromDays(155))),
                new Log(tools[3].Id, user3.Id, now.Subtract(TimeSpan.FromDays(50)), now.Subtract(TimeSpan.FromDays(42))),
                new Log(tools[3].Id, user1.Id, now.Subtract(TimeSpan.FromDays(39)), now.Subtract(TimeSpan.FromDays(20))),
                new Log(tools[3].Id, user2.Id, now.Subtract(TimeSpan.FromDays(10)), now.Subtract(TimeSpan.FromDays(0))),
                new Log(tools[3].Id, user1.Id, now, now),
                new Log(tools[4].Id, user3.Id, now.Subtract(TimeSpan.FromDays(85)), now.Subtract(TimeSpan.FromDays(60))),
            };
            db.AddRange(logs);

            db.SaveChanges();
        }
    }
}
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
                if (db.Database.EnsureCreated())
                {
                    // Add roles
                    var administratorRole = new IdentityRole("Admin");
                    rm.CreateAsync(administratorRole).Wait();

                    // Add users
                    var administrator = new ApplicationUser { UserName = "admin@uia.no", Email = "admin@uia.no", ChangePassword = true };
                    um.CreateAsync(administrator, "Password1.").Wait();
                    um.AddToRoleAsync(administrator, "Admin");

                    db.Statuses.AddRange(new List<Status>{
                        new Status { StatusName = "På lager", Style = "color: #8EE783", Glyphicon = "glyphicon glyphicon-ok-sign", IsDeleteable = false, Id = Status.AVAILABLE },
                        new Status { StatusName = "Utlånt", Style = "color: #FA7D12", Glyphicon = "glyphicon glyphicon-remove-sign", IsDeleteable = false, Id = Status.BUSY  },
                        new Status { StatusName = "Ikke tilgjengelig", Style = "color: #F6D846", Glyphicon = "glyphicon glyphicon-minus-sign", IsDeleteable = false, Id = Status.UNAVAILABLE  }
                    });
                }
                
                db.SaveChanges();
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

            var user1 = new ApplicationUser { UserName = "ole@uia.no", Email = "ole@uia.no", ChangePassword = true, UserIdentifierCode = "1" };
            um.CreateAsync(user1, "Password1.");

            var user2 = new ApplicationUser { UserName = "dole@uia.no", Email = "dole@uia.no", ChangePassword = true, UserIdentifierCode = "5" };
            um.CreateAsync(user2, "Password1.");

            var user3 = new ApplicationUser { UserName = "doffen@uia.no", Email = "doffen@uia.no", ChangePassword = true };
            um.CreateAsync(user3, "Password1.");

            var statusAvailable = new Status { StatusName = "På lager", Style = "color: #8EE783", Glyphicon = "glyphicon glyphicon-ok-sign", IsDeleteable = false, Id = Status.AVAILABLE};
            db.Add(statusAvailable);

            var statusBusy = new Status { StatusName = "Utlånt", Style = "color: #FA7D12", Glyphicon = "glyphicon glyphicon-remove-sign", IsDeleteable = false, Id = Status.BUSY };
            db.Add(statusBusy);

            var statusUnavailable = new Status { StatusName = "Ikke tilgjengelig", Style = "color: #F6D846", Glyphicon = "glyphicon glyphicon-minus-sign", IsDeleteable = false, Id = Status.UNAVAILABLE };
            db.Add(statusUnavailable);

            // Add dummy data here
            var tools = new List<Tool>
            {

                new Tool("Hjelm med visir", statusAvailable, new List<Report>() { new Report(9, "Sprekk i visir.", user1.Id) }, new List<Alarm>(), "", "B1"),
                new Tool("Fallsele 1", statusAvailable, new List<Report>() { new Report(8, "Trenger snart et utbytte.", user1.Id) }, new List<Alarm>(), "", "B2"),
                new Tool("Fallsele 2", statusAvailable, new List<Report>(), new List<Alarm>(), "", "B3"),
                new Tool("C-presstang", statusAvailable, new List<Report>(), new List<Alarm>(), "", "B4"),
                new Tool("Termografisk kamera", statusAvailable, new List<Report>(), new List<Alarm>() {new Alarm("Kalibrering", new DateTime(2018,8,22))}, "apparat, kartlegging", "B5"),
                new Tool("Kabelskotang", statusAvailable, new List<Report>(), new List<Alarm>(), "", "B6"),
                new Tool("Nitetang", statusAvailable, new List<Report>(), new List<Alarm>(new Alarm[] { new Alarm("Fornye sertifikat", new DateTime(2018,9,13))}), "", "B7"),
                new Tool("AUS Koffert", statusAvailable, new List<Report>() { new Report(6, "Mangler 2 klypetenger.", user1.Id) }, new List<Alarm>(), "", "C1"),
                new Tool("Støvsuger", statusUnavailable, new List<Report>(), new List<Alarm>(), "", "C2"),
                new Tool("Presstang AL 240mm", statusAvailable, new List<Report>(), new List<Alarm>(), "", "C3"),
                new Tool("Skrujern", statusAvailable, new List<Report>(new Report[] { new Report(1, "Slitt skruhode.", admin.Id), new Report(1, "Sprekk i håndtak", user2.Id) }), new List<Alarm>(), "stjernejern", "A"),
                new Tool("Hammer", statusAvailable, new List<Report>() { new Report(2, "Håndtaket holder på å dette av.", user3.Id) }, new List<Alarm>(), "", "A"),
                new Tool("Sag", statusAvailable, new List<Report>(), new List<Alarm>(), "", "A"),
                new Tool("Vater", statusBusy, new List<Report>(), new List<Alarm>(), "måleutstyr", "A"),
                new Tool("Kniv", statusAvailable, new List<Report>(), new List<Alarm>(), "tapetkniv", "A")
            };

            var toolRFIDcode = 100;
            foreach (var tool in tools)
            {
                tool.Image = "images/example_tools/" + tool.Name.ToLower() + ".jpg";
                tool.Thumbnail = "images/example_tools/" + tool.Name.ToLower() + "_thumb.jpg";
                tool.ToolIdentifierCode = (toolRFIDcode++).ToString();
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
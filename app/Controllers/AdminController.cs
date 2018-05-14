using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using app.Data;
using app.Models;
using app.Models.AdminViewModels;

namespace app.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _um;
        private readonly ILogger _logger;

        public AdminController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> um,
            ILogger<AccountController> logger)
        {
            _context = context;
            _um = um;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new ToolStats();

            var toolUseCounts = 
                from l in _context.Logs
                group l by l.ToolId into grp
                orderby grp.Count() descending
                select new { tool = grp.First().Tool, count = grp.Count() };

            foreach (var line in toolUseCounts)
            {
                model.ToolUseCount.Add((line.tool, line.count));   
            }

            var yearAgo = DateTime.Now.Subtract(TimeSpan.FromDays(365));

            var logsByTool = _context.Logs
                .Where(l => l.ToDate > yearAgo)
                .Include(l => l.Tool)
                .GroupBy(l => l.ToolId);


            foreach (var logs in logsByTool)
            {
                double hours = 0.0;
                foreach (var log in logs)
                {
                    TimeSpan diff;
                    if (log.FromDate < yearAgo)
                        diff = log.ToDate - yearAgo;
                    else
                        diff = log.ToDate - log.FromDate;
                    hours += diff.TotalHours;
                }
                model.ToolUsage.Add((logs.First().Tool, hours / TimeSpan.FromDays(365).TotalHours));
            }

            return View(model);
        }

        public IActionResult Users()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email, ChangePassword = true };
                var result = await _um.CreateAsync(newUser, model.Password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Admin created a new account with password.");

                    return RedirectToAction(nameof(AdminController.Users));
                }

                AddErrors(result);
            }

            return View(model);
        }

        public async Task<IActionResult> Alarms()
        {
            return View(await _context.Alarms.ToListAsync());
        }

        public async Task<IActionResult> Reports()
        {
            return View(await _context.Reports.ToListAsync());
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}

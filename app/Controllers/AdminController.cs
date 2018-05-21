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

        public async Task <IActionResult> Index()
        {
            var model = new IndexViewModel();

            // Finds how many times each tool has been used
            var toolUseCounts = _context.Logs
                .Include(l => l.Tool)
                .GroupBy(l => l.ToolId)
                .Select(g => new {
                    tool = g.First().Tool, count = g.Count()
                })
                .OrderByDescending(t => t.count)
                .Take(10);

            foreach (var line in toolUseCounts)
            {
                model.ToolUseCount.Add((line.tool, line.count));   
            }

            // Finds how many
            var yearAgo = DateTime.Now.Subtract(TimeSpan.FromDays(365));

            var logsByTool = _context.Logs
                .Where(l => l.ToDate > yearAgo)
                .Include(l => l.Tool)
                .GroupBy(l => l.ToolId);

            var toolUsagePercent = new List<(Tool Tool, double UsagePercent)>();
            foreach (var logs in logsByTool)
            {
                double hours = 0.0;
                foreach (var log in logs)
                {
                    TimeSpan diff;
                    if (log.ToDate == log.FromDate)
                        diff = DateTime.Now - log.FromDate;
                    else if (log.FromDate < yearAgo)
                        diff = log.ToDate - yearAgo;
                    else
                        diff = log.ToDate - log.FromDate;
                    hours += diff.TotalHours;
                }
                toolUsagePercent.Add((logs.First().Tool, hours / TimeSpan.FromDays(365).TotalHours * 100));
            }

            model.ToolUsage.AddRange(toolUsagePercent.OrderByDescending(t => t.UsagePercent).Take(10));

            // Get the latest 10 logs
            model.Log10 = await _context.Logs
                .Include(l => l.Tool)
                .Include(l => l.User)
                .OrderByDescending(l => l.FromDate)
                .Take(10)
                .ToListAsync();

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

        [HttpGet]
        public IActionResult CreateManyUsers()
        {
            return View(new CreateManyUsersViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateManyUsers(CreateManyUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                string[] emails = model.Email.Split( new[] { Environment.NewLine }, StringSplitOptions.None );
                for (int i = 0; i < emails.Length; i++) {
                    var newUser = new ApplicationUser { UserName = emails[i], Email = emails[i], ChangePassword = true };
                    var result = await _um.CreateAsync(newUser, model.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Admin created a new account with password.");
                        continue;
                    }

                    AddErrors(result);
                }

                if (ModelState.ErrorCount == 0)
                    return RedirectToAction(nameof(AdminController.Users));
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

        public async Task<IActionResult> Log()
        {
            return View(await _context.Logs.OrderByDescending(l => l.FromDate).Include(l => l.User).Include(l => l.Tool).ToListAsync());
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

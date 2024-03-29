using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using app.Data;
using app.Models;

namespace app.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _um;

        public ReportController(ApplicationDbContext context, UserManager<ApplicationUser> um)
        {
            _context = context;
            _um = um;
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(int? id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .SingleOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            // Make sure the user is either an administrator or the owner of the report
            if (report.UserId != _um.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return BadRequest();
            }

            return View(report);
        }

        // GET: Reports/Create
        public IActionResult Create(int id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("Error")] Report report, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                report.UserId = _um.GetUserId(User);
                _context.Add(report);

                var tool = await _context.Tools.Include(t => t.Reports).SingleOrDefaultAsync(m => m.Id == id);
                if (tool == null)
                {
                    return NotFound();
                }

                tool.Reports.Add(report);

                await _context.SaveChangesAsync();
                
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                    
                return RedirectToAction(nameof(ToolController.Details), "Tool", new { id });
            }
            return View(report);
        }

        // GET: Reports/Edit/5
        public async Task<IActionResult> Edit(int? id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports.SingleOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            // Make sure the user is either an administrator or the owner of the report
            if (report.UserId != _um.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return BadRequest();
            }

            return View(report);
        }

        // POST: Reports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Error")] Report report, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id != report.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var pReport = await _context.Reports.AsNoTracking().SingleOrDefaultAsync(r => r.Id == id);

                try
                {
                    report.ToolId = pReport.ToolId;
                    report.UserId = pReport.UserId;
                    
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(nameof(ToolController.Details), "Tool", new { id = pReport.ToolId });
            }
            return View(report);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(int? id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .SingleOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            // Make sure the user is either an administrator or the owner of the report
            if (report.UserId != _um.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return BadRequest();
            }

            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var report = await _context.Reports.SingleOrDefaultAsync(m => m.Id == id);
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

            return RedirectToAction(nameof(ToolController.Details), "Tool", new { id = report.ToolId });
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}

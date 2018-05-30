using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using app.Data;
using app.Models;
using app.Models.AlarmViewModels;

namespace app.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AlarmsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlarmsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Alarms/Create
        public async Task<IActionResult> Create(int id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var statuslist = await _context.Statuses.ToListAsync();
            var vm = new CreateViewModel();
            vm.Alarm.ToolId = id;
            vm.Statuses = statuslist;
            return View(vm);
        }

        // POST: Alarms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("Name,Date,StatusId")] Alarm alarm, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                _context.Add(alarm);

                var tool = await _context.Tools.Include(t => t.Alarms).SingleOrDefaultAsync(m => m.Id == id);
                if (tool == null)
                {
                    return NotFound();
                }

                tool.Alarms.Add(alarm);

                await _context.SaveChangesAsync();
                
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(nameof(AdminController.Alarms), "Admin");
            }
            return View(alarm);
        }

        // GET: Alarms/Edit/5
        public async Task<IActionResult> Edit(int? id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id == null)
            {
                return NotFound();
            }

            var alarm = await _context.Alarms.SingleOrDefaultAsync(m => m.Id == id);
            if (alarm == null)
            {
                return NotFound();
            }
            var statuslist = await _context.Statuses.ToListAsync();
            var vm = new EditViewModel();
            vm.Alarm = alarm;
            vm.Statuses = statuslist;
            return View(vm);
        }

        // POST: Alarms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Date,StatusId")] Alarm alarm, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id != alarm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var pAlarm = await _context.Alarms.AsNoTracking().SingleOrDefaultAsync(r => r.Id == id);

                try
                {
                    alarm.ToolId = pAlarm.ToolId;

                    _context.Update(alarm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlarmExists(alarm.Id))
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

                return RedirectToAction(nameof(AdminController.Alarms), "Admin");
            }
            return View(alarm);
        }

        // GET: Alarms/Delete/5
        public async Task<IActionResult> Delete(int? id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id == null)
            {
                return NotFound();
            }

            var alarm = await _context.Alarms
                .SingleOrDefaultAsync(m => m.Id == id);
            if (alarm == null)
            {
                return NotFound();
            }

            return View(alarm);
        }

        // POST: Alarms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var alarm = await _context.Alarms.SingleOrDefaultAsync(m => m.Id == id);
            _context.Alarms.Remove(alarm);
            await _context.SaveChangesAsync();
            
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(AdminController.Alarms), "Admin");
        }

        private bool AlarmExists(int id)
        {
            return _context.Alarms.Any(e => e.Id == id);
        }
    }
}

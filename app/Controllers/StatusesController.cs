using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using app.Data;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using app.Models.StatusesViewModels;

namespace app.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatusesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatusesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Statuses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Statuses.ToListAsync());
        }

        // GET: Statuses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Statuses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StatusName,Style,Glyphicon")] Status status)
        {
            if (ModelState.IsValid)
            {
                _context.Add(status);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminController.Statuses), "Admin");
            }
            return View(status);
        }

        // GET: Statuses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _context.Statuses.SingleOrDefaultAsync(m => m.Id == id);
            if (status == null)
            {
                return NotFound();
            }
            return View(status);
        }

        // POST: Statuses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StatusName,Style,Glyphicon")] Status status)
        {
            if (id != status.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    status.IsDeleteable = (await _context.Statuses.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id)).IsDeleteable;
                    _context.Update(status);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusExists(status.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AdminController.Statuses), "Admin");
            }
            return View(status);
        }

        // GET: Statuses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _context.Statuses
                .SingleOrDefaultAsync(m => m.Id == id);
            if (status == null)
            {
                return NotFound();
            }

            var vm = new DeleteViewModel();

            vm.Status = status;

            vm.Statuses = await _context.Statuses.ToListAsync();

            vm.Tools = await _context.Tools.Where(t => t.Status.Id == status.Id).ToListAsync();

            vm.Alarms = await _context.Alarms.Where(a => a.Status.Id == status.Id).ToListAsync();

            return View(vm);
        }

        // POST: Statuses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int newStatus)
        {
            if (id == newStatus)
            {
                return BadRequest("Can't assign status flagged for deletion.");
            }
            var status = await _context.Statuses.SingleOrDefaultAsync(m => m.Id == id);
            if (!status.IsDeleteable)
            {
                return BadRequest("Can't delete undeleteable status.");
            }
            

            foreach(Tool tool in _context.Tools)
            {
                if (tool.StatusId == id)
                {
                    tool.StatusId = newStatus;
                }
            }
            foreach(Alarm alarm in _context.Alarms)
            {
                if (alarm.StatusId == id)
                {
                    alarm.StatusId = newStatus;
                }
            }

            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminController.Statuses), "Admin");
        }

        private bool StatusExists(int id)
        {
            return _context.Statuses.Any(e => e.Id == id);
        }
    }
}

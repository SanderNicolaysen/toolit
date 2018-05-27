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

namespace app.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            // Only the superAdmin can edit himself/herself
            if (applicationUser.isSuperAdmin)
            {
                if (!User.IsInRole("SuperAdmin"))
                    return BadRequest();
            }

            // Admins can only edit non-admins
            if (applicationUser.isAdmin)
            {
                if (!User.IsInRole("SuperAdmin"))
                    return BadRequest();        
            }

            return View(applicationUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,PhoneNumber,UserIdentifierCode")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            // Only the superAdmin can edit himself/herself
            if (applicationUser.isSuperAdmin)
            {
                if (!User.IsInRole("SuperAdmin"))
                    return BadRequest();
            }

            // Admins can only edit non-admins
            if (applicationUser.isAdmin)
            {
                if (!User.IsInRole("SuperAdmin"))
                    return BadRequest();        
            }

            if (ModelState.IsValid)
            {
                var pUser = await _context.Users.AsNoTracking().SingleOrDefaultAsync(r => r.Id == id);

                try
                {
                    pUser.UserName = applicationUser.UserName;
                    pUser.Email = applicationUser.Email;
                    pUser.PhoneNumber = applicationUser.PhoneNumber;
                    pUser.UserIdentifierCode = applicationUser.UserIdentifierCode;

                    _context.Update(pUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AdminController.Users), "Admin");
            }
            return View(applicationUser);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            // Nobody can delete the super-admin
            if (applicationUser.isSuperAdmin)
                return BadRequest();

            // Admins can only delete non-admins
            if (!User.IsInRole("SuperAdmin"))
            {
                if (applicationUser.isAdmin)
                    return BadRequest();
            }

            return View(applicationUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);

            // Nobody can delete the super-admin
            if (applicationUser.isSuperAdmin)
                return BadRequest();

            // Admins can only delete non-admins
            if (!User.IsInRole("SuperAdmin"))
            {
                if (applicationUser.isAdmin)
                    return BadRequest();
            }

            // Delete all of the logs which belong to this user
            var pLogs = await _context.Logs.AsNoTracking().Where(l => l.UserId == applicationUser.Id).ToListAsync();
            _context.Logs.RemoveRange(pLogs);

            // Delete all of the reports which belong to this user
            var pReports = await _context.Reports.AsNoTracking().Where(r => r.UserId == applicationUser.Id).ToListAsync();
            _context.Reports.RemoveRange(pReports);

            // Delete all of the reservations which belong to this user
            var pReservations = await _context.Reservations.AsNoTracking().Where(r => r.UserId == applicationUser.Id).ToListAsync();
            _context.Reservations.RemoveRange(pReservations);

            // Delete all of the favorites which belong to this user
            var pFavorites = await _context.Favorites.AsNoTracking().Where(f => f.UserId == applicationUser.Id).ToListAsync();
            _context.Favorites.RemoveRange(pFavorites);

            // Delete all of the notifications which belong to this user
            var pNotifications = await _context.Notifications.AsNoTracking().Where(n => n.UserId == applicationUser.Id).ToListAsync();
            _context.Notifications.RemoveRange(pNotifications);

            _context.Users.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminController.Users), "Admin");
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

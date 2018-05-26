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
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,PhoneNumber")] ApplicationUser applicationUser)
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
                    applicationUser.isAdmin = pUser.isAdmin;
                    applicationUser.isSuperAdmin = pUser.isSuperAdmin;

                    applicationUser.NormalizedUserName = applicationUser.UserName.ToUpper();
                    applicationUser.NormalizedEmail = applicationUser.Email.ToUpper();

                    applicationUser.EmailConfirmed = pUser.EmailConfirmed;
                    applicationUser.PasswordHash = pUser.PasswordHash;
                    applicationUser.SecurityStamp = pUser.SecurityStamp;
                    applicationUser.ConcurrencyStamp = pUser.ConcurrencyStamp;
                    applicationUser.PhoneNumberConfirmed = pUser.PhoneNumberConfirmed;
                    applicationUser.TwoFactorEnabled = pUser.TwoFactorEnabled;
                    applicationUser.LockoutEnd = pUser.LockoutEnd;
                    applicationUser.LockoutEnabled = pUser.LockoutEnabled;
                    applicationUser.AccessFailedCount = pUser.AccessFailedCount;

                    _context.Update(applicationUser);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using app.Data;
using app.Models;
using app.Services;

namespace app.Controllers_Api
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationManager _nm;
        private readonly UserManager<ApplicationUser> _um;

        public UsersController(ApplicationDbContext context, INotificationManager nm, UserManager<ApplicationUser> um)
        {
            _context = context;
            _nm = nm;
            _um = um;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IEnumerable<ApplicationUser> GetApplicationUser()
        {
            return _context.Users.OrderByDescending(u => u.isAdmin);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetApplicationUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return Ok(applicationUser);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutApplicationUser([FromRoute] string id, [FromBody] ApplicationUser applicationUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != applicationUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(applicationUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostApplicationUser([FromBody] ApplicationUser applicationUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Users.Add(applicationUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplicationUser", new { id = applicationUser.Id }, applicationUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteApplicationUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            _context.Users.Remove(applicationUser);
            await _context.SaveChangesAsync();

            return Ok(applicationUser);
        }

        [Authorize(Roles = "Admin")]
        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // PUT: api/Users/5/markAsAdmin
        [HttpPut("{id}/markAsAdmin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> MarkAsAdmin([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (!user.isAdmin)
            {
                await _um.AddToRoleAsync(user, "Admin");

                user.isAdmin = true;

                _nm.SendNotificationAsync(user.Id, 
                "Du har blitt merket som en administrator!", 
                "/Admin").Wait();
            }
            else
            {
                await _um.RemoveFromRoleAsync(user, "Admin");

                user.isAdmin = false;

                _nm.SendNotificationAsync(user.Id, 
                "Du har blitt merket som en vanlig bruker!", 
                "/Tool").Wait();
            }

            _context.Update(user);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
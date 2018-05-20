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

namespace app.Controllers_Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Reservations")]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _um;

        public ReservationsController(ApplicationDbContext context, UserManager<ApplicationUser> um)
        {
            _context = context;
            _um = um;
        }

        // GET: api/Reservations
        [HttpGet]
        public IEnumerable<Reservation> GetReservation(int? toolid)
        {
            if (toolid != null)
            {
                var reservations = _context.Reservations
                    .Where(r => r.ToolId == toolid)
                    .Include(r => r.User);

                return reservations.ToList();
            }

            return _context.Reservations.Include(r => r.User);
        }

        // GET: api/Reservations/cal/5
        // Used by fullcalendar to retrieve events
        [HttpGet("cal/{id}")]
        public IEnumerable<object> GetReservation([FromRoute] int id, string start, string end)
        {
            var Start = DateTime.Parse(start, null, System.Globalization.DateTimeStyles.RoundtripKind);
            var End = DateTime.Parse(end, null, System.Globalization.DateTimeStyles.RoundtripKind);

            var reservations = _context.Reservations
                .Where(r => r.ToolId == id)
                .Where(r => r.ToDate > Start && r.FromDate < End)
                .Include(r => r.User)
                .Select(r => new { title = r.User.UserName, start = r.FromDate, end = r.ToDate, id = r.Id, user = r.User });

            return reservations;
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reservation = await _context.Reservations.SingleOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        // PUT: api/Reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation([FromRoute] int id, [FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reservation.Id)
            {
                return BadRequest();
            }

            var res = await _context.Reservations.AsNoTracking().SingleOrDefaultAsync(r => r.Id == id);
            // Make sure the user is either an administrator or the owner of the reservation
            if (res.UserId != _um.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return BadRequest();
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
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

        // POST: api/Reservations
        [HttpPost]
        public async Task<IActionResult> PostReservation([FromBody][Bind("User.UserName", "ToolId", "FromDate", "ToDate")] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!String.IsNullOrEmpty(reservation.User.UserName) && User.IsInRole("Admin"))
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == reservation.User.UserName);
                if (user == null)
                {
                    return BadRequest(ModelState);
                }
                reservation.UserId = user.Id;
            }
            else
            {
                reservation.UserId = _um.GetUserId(User);
            }
            reservation.User = null;

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reservation = await _context.Reservations.SingleOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Make sure the user is either an administrator or the owner of the reservation
            if (reservation.UserId != _um.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return BadRequest();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
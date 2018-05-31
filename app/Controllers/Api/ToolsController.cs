using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using app.Data;
using app.Models;

namespace app.Controllers_Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Tools")]
    public class ToolsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToolsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Tools
        [HttpGet]
        public IEnumerable<Object> GetTools()
        {
            var n = _context.Tools
            .Include(tool => tool.Status)
            .GroupJoin(
                _context.Users, 
                t => t.CurrentOwnerId, 
                u => u.Id, 
                (t, u) => new { Tool = t, User = u})
            .SelectMany(
                u => u.User.DefaultIfEmpty(),
                (t, u) => new {
                    Tool = t.Tool, User = u
                }
            );

            return n;
        }

        // GET: api/Tools/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTool([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tool = await _context.Tools.Include(t => t.Status).SingleOrDefaultAsync(m => m.Id == id);

            if (tool == null)
            {
                return NotFound();
            }

            return Ok(tool);
        }

        // PUT: api/Tools/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTool([FromRoute] int id, [FromBody] Tool tool)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tool.Id)
            {
                return BadRequest();
            }

            _context.Entry(tool).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToolExists(id))
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

        // POST: api/Tools
        [HttpPost]
        public async Task<IActionResult> PostTool([FromBody] Tool tool)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tools.Add(tool);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTool", new { id = tool.Id }, tool);
        }

        // DELETE: api/Tools/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTool([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tool = await _context.Tools.SingleOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            _context.Tools.Remove(tool);
            await _context.SaveChangesAsync();

            return Ok(tool);
        }

        // PUT: api/Tools/5/checkout
        [HttpPut("{id}/checkout")]
        public async Task<IActionResult> CheckoutTool([FromRoute] int id, [FromServices] UserManager<ApplicationUser> um)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tool = await _context.Tools.Include(s => s.Status).SingleOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            if (tool.CurrentOwnerId != "No owner")
            {
                return BadRequest("Tool is already checked out.");
            }

            tool.StatusId = Status.BUSY;

            var userId = (await um.GetUserAsync(User)).Id;
            tool.CurrentOwnerId = userId;
            _context.Update(tool);

            var now = DateTime.UtcNow;
            _context.Logs.Add(new Log(tool.Id, userId, now, now));

            await _context.SaveChangesAsync();

            return Ok(tool);
        }

        // PUT: api/Tools/5/checkout
        [HttpPut("{id}/checkin")]
        public async Task<IActionResult> CheckinTool([FromRoute] int id, [FromServices] UserManager<ApplicationUser> um)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tool = await _context.Tools.Include(t => t.Status).SingleOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            tool.StatusId = Status.AVAILABLE;

            var userId = (await um.GetUserAsync(User)).Id;
            if (tool.CurrentOwnerId != userId)
            {
                return BadRequest("Tool is checked out by someone else.");
            }

            tool.CurrentOwnerId = "No owner";
            _context.Update(tool);

            var logEntry = await _context.Logs.Where(l => l.ToolId == id).OrderByDescending(l => l.Id).FirstAsync();
            logEntry.ToDate = DateTime.UtcNow;
            _context.Update(logEntry);

            await _context.SaveChangesAsync();

            return Ok(tool);
        }

        private bool ToolExists(int id)
        {
            return _context.Tools.Any(e => e.Id == id);
        }
    }
}
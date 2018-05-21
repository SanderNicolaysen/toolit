using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using app.Data;
using app.Models;
using app.Services;

namespace app.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Report")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationManager _nm;

        public ReportsController(ApplicationDbContext context, INotificationManager nm)
        {
            _context = context;
            _nm = nm;
        }

        // GET: api/Report
        [HttpGet]
        public IEnumerable<Report> GetReports()
        {
            return _context.Reports.Include(r => r.Tool).Include(r => r.User).OrderBy(r => r.isResolved);
        }

        // GET: api/Report/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await _context.Reports.SingleOrDefaultAsync(m => m.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        // PUT: api/Report/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReport([FromRoute] int id, [FromBody] Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != report.Id)
            {
                return BadRequest();
            }

            _context.Entry(report).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(id))
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

        // POST: api/Report
        [HttpPost]
        public async Task<IActionResult> PostReport([FromBody] Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReport", new { id = report.Id }, report);
        }

        // DELETE: api/Report/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await _context.Reports.SingleOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return Ok(report);
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }

        // PUT: api/Report/5/markAsResolved
        [HttpPut("{id}/markAsResolved")]
        public async Task<IActionResult> MarkAsResolved([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await _context.Reports.SingleOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            if (!report.isResolved)
            {
                report.isResolved = true;

                _nm.SendNotificationAsync(report.UserId, 
                "Rapporten '" + report.Error + "' har blitt merket som løst!", 
                "/Tool/Details/" + report.ToolId).Wait();
            }
            else
            {
                report.isResolved = false;

                _nm.SendNotificationAsync(report.UserId, 
                "Rapporten '" + report.Error + "' har blitt merket som uløst!", 
                "/Tool/Details/" + report.ToolId).Wait();
            }

            _context.Update(report);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
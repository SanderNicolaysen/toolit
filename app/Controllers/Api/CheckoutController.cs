using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using app.Models;
using app.Data;

namespace app.Controllers_Api
{
    [Produces("application/json")]
    [Route("api/checkout")]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _um;

        public CheckoutController(UserManager<ApplicationUser> um, ApplicationDbContext context)
        {
            _um = um;
            _context = context;
        }

        public class RequestBody
        {
            public string token;
            public string userIdentifierCode;
            public string toolIdentifierCode;
        }

        [HttpPost]
        public async Task<IActionResult> Interface([FromBody] RequestBody requestBody)
        {
            if (requestBody.token != "ABCabc123")
            {
                return BadRequest("Invalid api token");
            }

            var tool = await _context.Tools.SingleOrDefaultAsync(t => t.ToolIdentifierCode == requestBody.toolIdentifierCode);
            if (tool == null)
            {
                return BadRequest(new { toolIdentifierCode = "Not a valid identifier code for a tool."});
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserIdentifierCode == requestBody.userIdentifierCode);
            if (user == null)
            {
                return BadRequest(new { userIdentifierCode = "Not a valid identifier code for a user."});
            }

            if (tool.CurrentOwnerId == "No owner")
            {
                return await Take(user, tool);
            }
            
            return await Put(user, tool);
        }


        private async Task<IActionResult> Take(ApplicationUser user, Tool tool)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var date = DateTime.Now;

            var logEntry = new Log(tool.Id, user.Id, date, date);
            _context.Add(logEntry);

            tool.CurrentOwnerId = user.Id;
            _context.Update(tool);

            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<IActionResult> Put(ApplicationUser user, Tool tool)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var date = DateTime.Now;

            tool.CurrentOwnerId = "No owner";
            _context.Update(tool);

            var logEntry = await _context.Logs.Where(l => l.ToolId == tool.Id).OrderByDescending(l => l.Id).FirstAsync();
            logEntry.ToDate = DateTime.UtcNow;
            _context.Update(logEntry);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
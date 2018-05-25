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
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _um;

        public CheckoutController(UserManager<ApplicationUser> um, ApplicationDbContext context)
        {
            _um = um;
            _context = context;

            var user = _context.Users.First();
            _um.IsInRoleAsync(user, "Admin");
        }

        public class ApiRequest
        {
            public string key;
            public string userIdentifierCode;
            public string toolIdentifierCode;
        }

        [HttpPost("api/echo")]
        public IActionResult CheckApiKey([FromBody][Bind("key")] ApiRequest request)
        {
            if (!ValidateApiRequest(request))
            {
                return BadRequest("Invalid request key");
            }

            return Ok("Valid request key");
        }

        [HttpPost("api/checkuser")]
        public async Task<IActionResult> CheckUser([FromBody][Bind("key, userIdentifierCode")] ApiRequest request)
        {
            if (!ValidateApiRequest(request))
            {
                return BadRequest("Invalid request key");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserIdentifierCode == request.userIdentifierCode);

            if (user == null)
            {
                return NotFound();
            }
            
            return Ok(user.UserName);
        }

        [HttpPost("api/checktool")]
        public async Task<IActionResult> CheckTool([FromBody][Bind("key, toolIdentifierCode")] ApiRequest request)
        {
            if (!ValidateApiRequest(request))
            {
                return BadRequest("Invalid request key");
            }

            var tool = await _context.Tools.SingleOrDefaultAsync(t => t.ToolIdentifierCode == request.toolIdentifierCode);

            if (tool == null)
            {
                return NotFound();
            }
            
            return Ok(tool.Name);
        }

        [HttpPost("api/checkout")]
        [HttpPost("api/checkin")]
        public async Task<IActionResult> Interface([FromBody] ApiRequest request)
        {
            if (!ValidateApiRequest(request))
            {
                return BadRequest("Invalid request key");
            }

            var tool = await _context.Tools.SingleOrDefaultAsync(t => t.ToolIdentifierCode == request.toolIdentifierCode);
            if (tool == null)
            {
                return BadRequest(new { toolIdentifierCode = "Not a valid identifier code for a tool."});
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserIdentifierCode == request.userIdentifierCode);
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

            return Ok(tool.Name + " checked out.");
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

            return Ok(tool.Name + " checked in.");;
        }

        private bool ValidateApiRequest(ApiRequest request)
        {
            return _context.ApiKeys.Any(api => api.Key == request.key);
        }
    }
}
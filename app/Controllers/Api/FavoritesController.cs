using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Data;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace app.Controllers_Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Favorites")]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _um;

        public FavoritesController(ApplicationDbContext context, UserManager<ApplicationUser> um)
        {
            _context = context;
            _um = um;
        }

        // GET: api/Favorites
        [HttpGet]
        public async Task<IEnumerable<Favorite>> GetFavorites()
        {
            return await _context.Favorites.Where(f => f.UserId == _um.GetUserId(User)).ToListAsync();
        }

        // POST: api/Favorites
        [HttpPost]
        public async Task<IActionResult> PostFavorite([FromBody][Bind("ToolId")] Favorite favorite)
        {
            if (!ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == _um.GetUserId(User));
            var tool = await _context.Tools.SingleOrDefaultAsync(t => t.Id == favorite.ToolId);
            _context.Favorites.Add(new Favorite{UserId=user.Id, ToolId=tool.Id});
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFavorites", new { id = favorite.ToolId });
        }

        // DELETE: api/Favorites
        [HttpDelete]
        public async Task<IActionResult> DeleteFavorite([FromBody][Bind("ToolId")] Favorite favorite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == _um.GetUserId(User));
            if (user == null)
            {
                return NotFound();
            }

            var fav = await _context.Favorites.Where(f => f.UserId == user.Id).SingleOrDefaultAsync(f => f.ToolId == favorite.ToolId);
            _context.Favorites.Remove(fav);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
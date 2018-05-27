using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

using app.Data;
using app.Models;
using app.Models.ToolViewModels;
using app.Services;

namespace app.Controllers
{
    [Authorize]
    public class ToolController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ToolController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Tool
        public async Task<IActionResult> Index()
        {
            var a = await _context.Tools.Include(tool => tool.Reports).Include(tool => tool.Status).ToListAsync();
            return View(a);
        }

        // GET: Tool/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool = await _context.Tools
                .Include(t => t.Reports)
                .Include(t => t.Alarms)
                .Include(t => t.Status)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();

            return View(tool);
        }

        // GET: Tool/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var statuslist = await _context.Statuses.ToListAsync();
            var vm = new CreateViewModel();
            vm.Statuses = statuslist;
            return View(vm);
        }

        // POST: Tool/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,StatusId,Alias,ToolIdentifierCode")] Tool tool, IFormFile image, [FromServices] IResizeImage _ri)
        {
            if (!ModelState.IsValid)
            {
                return View(tool);
            }

            if (image != null)
            {
                var imgs = _ri.GetImagePathsWithThumbnail(image);
                tool.Image = imgs.image;
                tool.Thumbnail = imgs.thumbnail;
            }

            _context.Add(tool);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Tool/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool = await _context.Tools.SingleOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            var vm = new EditViewModel();
            vm.Tool = tool;
            vm.Statuses = await _context.Statuses.ToListAsync();

            return View(vm);
        }

        // POST: Tool/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StatusId,Alias,ToolIdentifierCode")] Tool tool, IFormFile image, [FromServices] IResizeImage _ri)
        {
            if (id != tool.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(tool);
            }

            var pTool = await _context.Tools.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);

            // If an image is submitted replace the old one
            if (image != null)
            {
                // Delete previous images if they exist
                {
                    if (pTool != null && pTool.Image != null)
                        System.IO.File.Delete(Path.Combine(_hostingEnvironment.WebRootPath, pTool.Image));
                    if (pTool != null && pTool.Thumbnail != null)                        
                        System.IO.File.Delete(Path.Combine(_hostingEnvironment.WebRootPath, pTool.Thumbnail));
                }

                var imgs = _ri.GetImagePathsWithThumbnail(image);
                tool.Image = imgs.image;
                tool.Thumbnail = imgs.thumbnail;
            }

            // otherwise, make sure to keep the old image paths!
            else
            {
                tool.Image = pTool.Image;
                tool.Thumbnail = pTool.Thumbnail;
                tool.CurrentOwnerId = pTool.CurrentOwnerId;
            }

            try
            {
                _context.Update(tool);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToolExists(tool.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Tool/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool = await _context.Tools
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            return View(tool);
        }

        // POST: Tool/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tool = await _context.Tools
            .Include(t => t.Reports)
            .Include(t => t.Alarms)
            .SingleOrDefaultAsync(m => m.Id == id);
            _context.Tools.Remove(tool);

            foreach (var report in tool.Reports)
            {
                _context.Reports.Remove(report);
            }

            foreach (var alarm in tool.Alarms)
            {
                _context.Alarms.Remove(alarm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool ToolExists(int id)
        {
            return _context.Tools.Any(e => e.Id == id);
        }

        // GET: Tool/Report/5
        public async Task<IActionResult> Report(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tool = await _context.Tools.SingleOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }
            return View(tool);
        }

        // POST: Tool/Report/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(int id, [Bind("Id,Report")] Tool tool)
        {
            if (id != tool.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tool);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToolExists(tool.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tool);
        }
    }
}

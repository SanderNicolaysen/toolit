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
            return View(await _context.Tools.Include(tool => tool.Reports).ToListAsync());
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
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tool == null)
            {
                return NotFound();
            }

            DateTime localTime = DateTime.Now;

            tool.Status = "Available";

            // If at least one of the alarm-dates have been surpassed, change the tool-status to "Not available" 
            foreach (var alarm in tool.Alarms)
            {
                if (alarm.Date.CompareTo(localTime) < 0)
                    tool.Status = "Not available";
            }

            await _context.SaveChangesAsync();

            return View(tool);
        }

        // GET: Tool/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tool/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Status")] Tool tool, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return View(tool);
            }

            var filename = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(image.FileName);
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "images\\uploads\\" + filename);
            //var fileStream = System.IO.File.Create(path);            
            //image.OpenReadStream().CopyTo(fileStream);
            //fileStream.Close();

            using (Image<Rgba32> img = Image.Load(image.OpenReadStream())) //open the file and detect the file type and decode it
            {
                // image is now in a file format agnostic structure in memory as a series of Rgba32 pixels
                img.Mutate(ctx => ctx.Resize(100, 0)); // resize the image in place and return it for chaining
                img.Save(path); // based on the file extension pick an encoder then encode and write the data to disk
            } // dispose - releasing memory into a memory pool ready for the next image you wish to process

            tool.ImagePath = Path.GetRelativePath(_hostingEnvironment.WebRootPath, path);
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
            return View(tool);
        }

        // POST: Tool/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status")] Tool tool)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HighlanderMaster.Models;
using Microsoft.AspNetCore.Authorization;

namespace HighlanderMaster.Controllers
{
    public class TripRoutesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TripRoutesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(RouteCategory? category)
        {
            var routes = _context.Routes.AsQueryable();

            if (category != null)
            {
                routes = routes.Where(r => r.Category == category);
            }

            return View(await routes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tripRoute = await _context.Routes
                .Include(r => r.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tripRoute == null) return NotFound();

            return View(tripRoute);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(TripRoute tripRoute, IFormFile mainPicFile, List<IFormFile> pictureFiles)
        {
            if (mainPicFile != null && mainPicFile.Length > 0)
                tripRoute.MainPicURL = await SaveFile(mainPicFile);

            if (pictureFiles != null && pictureFiles.Count > 0)
            {
                var paths = new List<string>();
                foreach (var file in pictureFiles)
                    if (file.Length > 0)
                        paths.Add(await SaveFile(file));
                tripRoute.PicturesURL = string.Join(",", paths);
            }

            ModelState.Remove("MainPicURL");
            ModelState.Remove("PicturesURL");
            ModelState.Remove("Comments");
            ModelState.Remove("mainPicFile");
            ModelState.Remove("pictureFiles");

            if (ModelState.IsValid)
            {
                _context.Add(tripRoute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tripRoute);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Routes == null)
                return NotFound();

            var tripRoute = await _context.Routes.FindAsync(id);
            if (tripRoute == null)
                return NotFound();

            return View(tripRoute);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, TripRoute tripRoute, IFormFile mainPicFile, List<IFormFile> pictureFiles)
        {
            if (id != tripRoute.Id)
                return NotFound();

            var existing = await _context.Routes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

            if (mainPicFile == null || mainPicFile.Length == 0)
                tripRoute.MainPicURL = existing?.MainPicURL;
            else
                tripRoute.MainPicURL = await SaveFile(mainPicFile);

            if (pictureFiles == null || pictureFiles.Count == 0 || pictureFiles.All(f => f.Length == 0))
                tripRoute.PicturesURL = existing?.PicturesURL;
            else
            {
                var paths = new List<string>();
                foreach (var file in pictureFiles)
                    if (file.Length > 0)
                        paths.Add(await SaveFile(file));
                tripRoute.PicturesURL = string.Join(",", paths);
            }

            ModelState.Remove("MainPicURL");
            ModelState.Remove("PicturesURL");
            ModelState.Remove("Comments");
            ModelState.Remove("mainPicFile");
            ModelState.Remove("pictureFiles");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tripRoute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripRouteExists(tripRoute.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(tripRoute);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Routes == null)
                return NotFound();

            var tripRoute = await _context.Routes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tripRoute == null)
                return NotFound();

            return View(tripRoute);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Routes == null)
                return Problem("Entity set 'ApplicationDbContext.Routes' is null.");

            var tripRoute = await _context.Routes.FindAsync(id);
            if (tripRoute != null)
                _context.Routes.Remove(tripRoute);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + fileName;
        }

        private bool TripRouteExists(int id)
        {
            return (_context.Routes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

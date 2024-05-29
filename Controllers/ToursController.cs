using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseWork.Data;
using CourseWork.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace CourseWork.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class ToursController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public ToursController(DataContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Tours
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Tours.Include(t => t.artists);
            return View(await dataContext.ToListAsync());
        }

        // GET: Tours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tours == null)
            {
                return NotFound();
            }

            var tours = await _context.Tours
                .Include(t => t.artists)
                .FirstOrDefaultAsync(m => m.id == id);
            if (tours == null)
            {
                return NotFound();
            }

            return View(tours);
        }
        [Authorize(Roles = "admin")]
        // GET: Tours/Create
        public IActionResult Create()
        {
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name");
            return View();
        }
        [Authorize(Roles = "admin")]
        // POST: Tours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,Photo,artistsId")] Tours tours, IFormFile upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/files/images/tours/" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    tours.Photo = path;
                }
                _context.Add(tours);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name", tours.artistsId);
            return View(tours);
        }
        [Authorize(Roles = "admin")]
        // GET: Tours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tours == null)
            {
                return NotFound();
            }

            var tours = await _context.Tours.FindAsync(id);
            if (!tours.Photo.IsNullOrEmpty())
            {
                byte[] photodata =
                System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + tours.Photo);
                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
            }
            if (tours == null)
            {
                return NotFound();
            }
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name", tours.artistsId);
            return View(tours);
        }
        [Authorize(Roles = "admin")]
        // POST: Tours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,Photo,artistsId")] Tours tours, IFormFile upload)
        {
            if (id != tours.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/files/images/tours" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    if (!tours.Photo.IsNullOrEmpty())
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath +
                       tours.Photo);
                    }
                    tours.Photo = path;
                }
                try
                {
                    _context.Update(tours);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToursExists(tours.id))
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
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name", tours.artistsId);
            return View(tours);
        }
        [Authorize(Roles = "admin")]
        // GET: Tours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tours == null)
            {
                return NotFound();
            }

            var tours = await _context.Tours
                .Include(t => t.artists)
                .FirstOrDefaultAsync(m => m.id == id);
            if (tours == null)
            {
                return NotFound();
            }

            return View(tours);
        }
        [Authorize(Roles = "admin")]
        // POST: Tours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tours == null)
            {
                return Problem("Entity set 'DataContext.Tours'  is null.");
            }
            var tours = await _context.Tours.FindAsync(id);
            if (tours != null)
            {
                _context.Tours.Remove(tours);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToursExists(int id)
        {
          return (_context.Tours?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}

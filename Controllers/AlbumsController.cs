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
    public class AlbumsController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public AlbumsController(DataContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Albums.Include(a => a.artists).Include(a => a.genres);
            return View(await dataContext.ToListAsync());
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var albums = await _context.Albums
                .Include(a => a.artists)
                .Include(a => a.genres)
                .FirstOrDefaultAsync(m => m.id == id);

            if (albums == null)
            {
                return NotFound();
            }

            // Получаем все треки для данного альбома
            var tracks = await _context.Tracks
                .Where(t => t.albumsId == id)
                .ToListAsync();

            // Возвращаем модель с информацией об альбоме и его треках
            ViewBag.Tracks = tracks; // Можно также передать треки в модель представления через ViewBag или ViewModel

            return View(albums);
        }
        [Authorize(Roles = "admin")]
        // GET: Albums/Create
        public IActionResult Create()
        {
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name");
            ViewData["genresId"] = new SelectList(_context.Genres, "id", "name");
            return View();
        }
        [Authorize(Roles = "admin")]
        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,description,Photo,genresId,artistsId")] Albums albums, IFormFile upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/files/images/albums/" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    albums.Photo = path;
                }
                albums.date = DateTime.Now;
                _context.Add(albums);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name", albums.artistsId);
            ViewData["genresId"] = new SelectList(_context.Genres, "id", "name", albums.genresId);
            return View(albums);
        }
        [Authorize(Roles = "admin")]
        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var albums = await _context.Albums.FindAsync(id);
            if (!albums.Photo.IsNullOrEmpty())
            {
                byte[] photodata =
                System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + albums.Photo);
                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
            }
            if (albums == null)
            {
                return NotFound();
            }
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name", albums.artistsId);
            ViewData["genresId"] = new SelectList(_context.Genres, "id", "name", albums.genresId);
            return View(albums);
        }
        [Authorize(Roles = "admin")]
        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,description,date,Photo,genresId,artistsId")] Albums albums, IFormFile upload)
        {
            if (id != albums.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/files/images/albums" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    if (!albums.Photo.IsNullOrEmpty())
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath +
                       albums.Photo);
                    }
                    albums.Photo = path;
                }
                try
                {
                    _context.Update(albums);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumsExists(albums.id))
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
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name", albums.artistsId);
            ViewData["genresId"] = new SelectList(_context.Genres, "id", "name", albums.genresId);
            return View(albums);
        }
        [Authorize(Roles = "admin")]
        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var albums = await _context.Albums
                .Include(a => a.artists)
                .Include(a => a.genres)
                .FirstOrDefaultAsync(m => m.id == id);
            if (albums == null)
            {
                return NotFound();
            }

            return View(albums);
        }
        [Authorize(Roles = "admin")]
        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Albums == null)
            {
                return Problem("Entity set 'DataContext.Albums'  is null.");
            }
            var albums = await _context.Albums.FindAsync(id);
            if (albums != null)
            {
                _context.Albums.Remove(albums);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumsExists(int id)
        {
          return (_context.Albums?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}

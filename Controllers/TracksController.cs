using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseWork.Data;
using CourseWork.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace CourseWork.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class TracksController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public TracksController(DataContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult DownloadTrack(int id)
        {
            // Получить путь к аудиофайлу трека и путь к фотографии альбома по его идентификатору id
            var audioFilePath = GetAudioFilePathById(id);

            if (audioFilePath != null)
            {
                var audioFileName = Path.GetFileName(audioFilePath);
                var audioContentType = "audio/mpeg"; // MIME-тип аудиофайла, предположим, что это MP3

                // Возвращаем аудиофайл
                return File(audioFilePath, audioContentType, audioFileName);
            }
            else
            {
                // В случае, если трек не найден, вернуть NotFound
                return NotFound();
            }
        }

        public string GetAudioFilePathById(int id)
        {
            // Предположим, что у вас есть коллекция треков tracks, в которой вы можете найти трек по его идентификатору.
            var track = _context.Tracks.FirstOrDefault(t => t.id == id);

            if (track != null)
            {
                return track.file; // Возвращает путь к файлу трека из модели трека
            }
            else
            {
                // Если трек с указанным идентификатором не найден, возвращаем null или пустую строку
                return null;
            }
        }

        // GET: Tracks
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Tracks.Include(t => t.albums).Include(t => t.artists).Include(t => t.genres);
            return View(await dataContext.ToListAsync());
        }

        // GET: Tracks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tracks == null)
            {
                return NotFound();
            }

            var tracks = await _context.Tracks
                .Include(t => t.albums)
                .Include(t => t.artists)
                .Include(t => t.genres)
                .FirstOrDefaultAsync(m => m.id == id);
            if (tracks == null)
            {
                return NotFound();
            }

            return View(tracks);
        }
        [Authorize(Roles = "admin")]
        // GET: Tracks/Create
        public IActionResult Create()
        {
            ViewData["albumsId"] = new SelectList(_context.Albums, "id", "name");
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name");
            ViewData["genresId"] = new SelectList(_context.Genres, "id", "name");
            return View();
        }
        [Authorize(Roles = "admin")]
        // POST: Tracks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,file,genresId,albumsId,artistsId")] Tracks tracks, IFormFile upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/files/songs/" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    tracks.file = path;
                }
                tracks.date = DateTime.Now;
                _context.Add(tracks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["albumsId"] = new SelectList(_context.Albums, "id", "name", tracks.albumsId);
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name", tracks.artistsId);
            ViewData["genresId"] = new SelectList(_context.Genres, "id", "name", tracks.genresId);
            return View(tracks);
        }
        [Authorize(Roles = "admin")]
        // GET: Tracks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tracks == null)
            {
                return NotFound();
            }

            var tracks = await _context.Tracks.FindAsync(id);
            if (tracks == null)
            {
                return NotFound();
            }
            ViewData["albumsId"] = new SelectList(_context.Albums, "id", "name", tracks.albumsId);
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name", tracks.artistsId);
            ViewData["genresId"] = new SelectList(_context.Genres, "id", "name", tracks.genresId);
            return View(tracks);
        }
        [Authorize(Roles = "admin")]
        // POST: Tracks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,date,file,genresId,albumsId,artistsId")] Tracks tracks, IFormFile upload)
        {
            if (id != tracks.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/files/songs/" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    if (!tracks.file.IsNullOrEmpty())
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath +
                       tracks.file);
                    }
                    tracks.file = path;
                }
                try
                {
                    _context.Update(tracks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TracksExists(tracks.id))
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
            ViewData["albumsId"] = new SelectList(_context.Albums, "id", "name", tracks.albumsId);
            ViewData["artistsId"] = new SelectList(_context.Artists, "id", "name", tracks.artistsId);
            ViewData["genresId"] = new SelectList(_context.Genres, "id", "name", tracks.genresId);
            return View(tracks);
        }
        [Authorize(Roles = "admin")]
        // GET: Tracks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tracks == null)
            {
                return NotFound();
            }

            var tracks = await _context.Tracks
                .Include(t => t.albums)
                .Include(t => t.artists)
                .Include(t => t.genres)
                .FirstOrDefaultAsync(m => m.id == id);
            if (tracks == null)
            {
                return NotFound();
            }

            return View(tracks);
        }
        [Authorize(Roles = "admin")]
        // POST: Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tracks == null)
            {
                return Problem("Entity set 'DataContext.Tracks'  is null.");
            }
            var tracks = await _context.Tracks.FindAsync(id);
            if (tracks != null)
            {
                _context.Tracks.Remove(tracks);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TracksExists(int id)
        {
          return (_context.Tracks?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}

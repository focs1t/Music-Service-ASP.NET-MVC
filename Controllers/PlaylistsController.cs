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
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using CourseWork.Areas.Identity.Data;
using System.IO.Compression;

namespace CourseWork.Controllers
{
    [Authorize(Roles = "user, admin")]
    public class PlaylistsController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        // Обновленный конструктор
        public PlaylistsController(DataContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        //public string GetAudioFilePathById(int? tracksId)
        //{
        //    if (tracksId == null)
        //    {
        //        return null;
        //    }

        //    var track = _context.Tracks.FirstOrDefault(t => t.id == tracksId);

        //    if (track != null)
        //    {
        //        // Предположим, что у трека есть свойство FilePath, содержащее путь к аудиофайлу
        //        return track.file;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //// Метод для скачивания всех треков конкретного плейлиста
        //public async Task<IActionResult> DownloadPlaylistTracks(int id)
        //{
        //    var playlistTracks = await _context.TracksPlaylists
        //        .Include(tp => tp.tracks)
        //        .Where(tp => tp.playlistsId == id)
        //        .ToListAsync();

        //    if (playlistTracks.Count == 0)
        //    {
        //        return NotFound();
        //    }

        //    var tempFolderPath = Path.Combine(_appEnvironment.WebRootPath, "temp");
        //    Directory.CreateDirectory(tempFolderPath);

        //    var zipFileName = $"playlist_{id}_tracks.zip";
        //    var zipFilePath = Path.Combine(tempFolderPath, zipFileName);

        //    using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
        //    {
        //        foreach (var playlistTrack in playlistTracks)
        //        {
        //            var trackFilePath = GetAudioFilePathById(playlistTrack.tracksId);
        //            if (!string.IsNullOrEmpty(trackFilePath) && System.IO.File.Exists(trackFilePath))
        //            {
        //                var trackFileName = Path.GetFileName(trackFilePath);
        //                var entry = zipArchive.CreateEntry(trackFileName);
        //                using (var entryStream = entry.Open())
        //                {
        //                    using (var fileStream = new FileStream(trackFilePath, FileMode.Open))
        //                    {
        //                        await fileStream.CopyToAsync(entryStream);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // Возвращаем zip-архив как файл для скачивания
        //    return File(zipFilePath, "application/zip", zipFileName);
        //}

        [Authorize(Roles = "user")]
        // GET: Playlists for current user
        public async Task<IActionResult> UserPlaylists()
        {
            // Получаем все плейлисты для текущего пользователя
            var playlists = await _context.Playlists
                .Where(p => p.username == User.Identity.Name)
                .ToListAsync();

            return View(playlists);
        }


        [Authorize(Roles = "user")]
        // GET: UserPlaylists/Edit/5
        public async Task<IActionResult> EditUserPlaylist(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists.FirstOrDefaultAsync(m => m.id == id && m.username == User.Identity.Name);
            if (playlist == null)
            {
                return NotFound();
            }
            return View(playlist);
        }
        [Authorize(Roles = "user")]
        // POST: UserPlaylists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserPlaylist(int id, [Bind("id,username,name,description,date")] Playlists playlist)
        {
            if (id != playlist.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistsExists(playlist.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UserPlaylists));
            }
            return View(playlist);
        }
        [Authorize(Roles = "user")]
        // GET: UserPlaylists/Details/5
        public async Task<IActionResult> DetailsUserPlaylist(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists.FirstOrDefaultAsync(m => m.id == id && m.username == User.Identity.Name);
            if (playlist == null)
            {
                return NotFound();
            }

            var tracks = await _context.TracksPlaylists
                .Include(tp => tp.tracks)
                    .ThenInclude(t => t.artists)
                .Where(tp => tp.playlistsId == id)
                .ToListAsync();

            ViewBag.Tracks = tracks;

            return View(playlist);
        }
        [Authorize(Roles = "user")]
        // GET: UserPlaylists/Delete/5
        public async Task<IActionResult> DeleteUserPlaylist(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists.FirstOrDefaultAsync(m => m.id == id && m.username == User.Identity.Name);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }
        [Authorize(Roles = "user")]
        // POST: UserPlaylists/Delete/5
        [HttpPost, ActionName("DeleteUserPlaylist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedUserPlaylist(int id)
        {
            if (_context.Playlists == null)
            {
                return Problem("Entity set 'DataContext.Playlists' is null.");
            }
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null && playlist.username == User.Identity.Name)
            {
                _context.Playlists.Remove(playlist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserPlaylists));
        }



        // GET: Playlists
        public async Task<IActionResult> Index()
        {
              return _context.Playlists != null ? 
                          View(await _context.Playlists.ToListAsync()) :
                          Problem("Entity set 'DataContext.Playlists'  is null.");
        }

        // GET: Playlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            /*if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlists = await _context.Playlists
                .FirstOrDefaultAsync(m => m.id == id);
            if (playlists == null)
            {
                return NotFound();
            }

            return View(playlists);*/
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(m => m.id == id);

            if (playlist == null)
            {
                return NotFound();
            }

            // Получаем треки из таблицы TracksPlaylists для конкретного плейлиста
            var tracks = await _context.TracksPlaylists
                .Include(tp => tp.tracks)
                    .ThenInclude(t => t.artists) // Включаем информацию об исполнителе
                .Where(tp => tp.playlistsId == id)
                .ToListAsync();

            // Фильтруем треки по пользователю, если необходимо
            if (User.Identity.Name != playlist.username)
            {
                tracks = tracks.Where(tp => tp.playlists.username == User.Identity.Name).ToList();
            }

            ViewBag.Tracks = tracks;

            return View(playlist);
        }
        // GET: Playlists/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Playlists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,username,name,description")] Playlists playlists)
        {
            if (ModelState.IsValid)
            {
                playlists.date = DateTime.Now;
                _context.Add(playlists);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlists);
        }
        [Authorize(Roles = "admin")]
        // GET: Playlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlists = await _context.Playlists.FindAsync(id);
            if (playlists == null)
            {
                return NotFound();
            }
            return View(playlists);
        }
        [Authorize(Roles = "admin")]
        // POST: Playlists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,username,name,description,date")] Playlists playlists)
        {
            if (id != playlists.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlists);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistsExists(playlists.id))
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
            return View(playlists);
        }
        [Authorize(Roles = "admin")]
        // GET: Playlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlists = await _context.Playlists
                .FirstOrDefaultAsync(m => m.id == id);
            if (playlists == null)
            {
                return NotFound();
            }

            return View(playlists);
        }
        [Authorize(Roles = "admin")]
        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Playlists == null)
            {
                return Problem("Entity set 'DataContext.Playlists'  is null.");
            }
            var playlists = await _context.Playlists.FindAsync(id);
            if (playlists != null)
            {
                _context.Playlists.Remove(playlists);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaylistsExists(int id)
        {
          return (_context.Playlists?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}

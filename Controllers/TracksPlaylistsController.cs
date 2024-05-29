using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseWork.Data;
using CourseWork.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseWork.Controllers
{
    public class TracksPlaylistsController : Controller
    {
        private readonly DataContext _context;

        public TracksPlaylistsController(DataContext context)
        {
            _context = context;
        }

        // GET: TracksPlaylists
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.TracksPlaylists.Include(t => t.playlists).Include(t => t.tracks).ThenInclude(t => t.artists);
            return View(await dataContext.ToListAsync());
        }

        // GET: TracksPlaylists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TracksPlaylists == null)
            {
                return NotFound();
            }

            var tracksPlaylists = await _context.TracksPlaylists
                .Include(tp => tp.playlists)
                .Include(tp => tp.tracks)
                .ThenInclude(t => t.artists)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracksPlaylists == null)
            {
                return NotFound();
            }

            return View(tracksPlaylists);
        }

        //// GET: TracksPlaylists/Create
        //public IActionResult Create()
        //{
        //    var playlists = _context.Playlists.ToList();
        //    var playlistsTracks = playlists.Select(playlists => new SelectListItem
        //    {
        //        Value = playlists.id.ToString(),
        //        //username = playlists.username,
        //        Text = $"{playlists.name} - {playlists.username}"
        //    });//.Where(p => p.username == User.Identity.Name)
        //       //.ToListAsync();
        //    ViewData["playlistsId"] = new SelectList(playlistsTracks, "Value", "Text");
        //    return View();
        //    //ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name");
        //    //ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name");
        //    //return View();
        //}

        // GET: TracksPlaylists/Create
        public async Task<IActionResult> Create(int id)
        {
            //var tracks = _context.Tracks.Include(t => t.artists).ToList();
            //var tracksPlaylists = tracks.Select(tracks => new SelectListItem
            //{
            //    Value = tracks.id.ToString(),
            //    Text = $"{tracks.name} - {tracks.artists.name}"
            //});
            //ViewData["tracksId"] = new SelectList(tracksPlaylists, "Value", "Text");
            var currentId = id;
            var tracks = _context.Tracks.Where(t => t.id == id).ToList();
            var tracksPlaylist = tracks.Select(track => new SelectListItem
            {
                Value = track.id.ToString(),
                Text = track.name
            }).ToList();
            ViewData["tracksId"] = new SelectList(tracksPlaylist, "Value", "Text");
            // Получение текущего пользователя
            var currentUser = User.Identity.Name;

            // Поиск всех плейлистов, принадлежащих текущему пользователю
            var playlists = _context.Playlists.Where(p => p.username == currentUser).ToList();

            // Создание списка SelectListItem для найденных плейлистов
            var playlistsTracks = playlists.Select(playlist => new SelectListItem
            {
                Value = playlist.id.ToString(),
                Text = $"{playlist.name} - {playlist.username}"
            }).ToList();

            // Добавление списка в ViewData
            ViewData["playlistsId"] = new SelectList(playlistsTracks, "Value", "Text");

            return View();
            //var track = await _context.Tracks.FindAsync(id);
            //if (track != null)
            //{
            //    ViewData["tracksId"] = new SelectList(new[] { track }.ToList(), "id", "name");
            //}

            //var playlists = _context.Playlists.ToList();
            //var playlistsTracks = playlists.Select(playlists => new SelectListItem
            //{
            //    Value = playlists.id.ToString(),
            //    //username = playlists.username,
            //    Text = $"{playlists.name} - {playlists.username}"
            //});//.Where(p => p.username == User.Identity.Name)
            //   //.ToListAsync();
            //ViewData["playlistsId"] = new SelectList(playlistsTracks, "Value", "Text");
            //return View();
            //ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name");
            //ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name");
            //return View();
        }

        // POST: TracksPlaylists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,tracksId,playlistsId")] TracksPlaylists tracksPlaylists)
        {
            //var tracks = await _context.Tracks.FindAsync(id);

            //if (tracks == null)
            //{
            //    ModelState.AddModelError("", "Invalid track selection.");
            //}

            if (ModelState.IsValid)
            {
                //tracksPlaylists.tracksId = id; // Assuming you meant to set tracksId here, not tracks.id
                _context.Add(tracksPlaylists);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name", tracksPlaylists.playlistsId);
            ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name", tracksPlaylists.tracksId);

            return View(tracksPlaylists);
            ////var tracks = _context.Tracks.Find(tracksPlaylists.tracksId);
            //if (ModelState.IsValid)
            //{
            //    tracksPlaylists.tracks.id = id;
            //    _context.Add(tracksPlaylists);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            ////if (tracks != null)
            ////{
            ////    List<Tracks> tracks1 = new List<Tracks>();
            ////    tracks1.Add(tracks);
            ////    ViewData["tracksId"] = new SelectList(tracks1, "id", "name");
            ////}
            ////var playlists = await _context.Playlists
            ////    .Where(p => p.username == User.Identity.Name)
            ////    .ToListAsync();
            ////ViewData["playlistsId"] = new SelectList(playlists, "id", "name");
            //ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name", tracksPlaylists.playlistsId);
            //ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name", tracksPlaylists.tracksId);
            ////ViewData["tracksId"] = new SelectList(tracks1, "id", "name");
            //return View(tracksPlaylists);
        }

        // GET: TracksPlaylists/Create
        //public IActionResult Create()
        //{
        //    var tracks = _context.Tracks.Include(t => t.artists).ToList();
        //    var tracksPlaylists = tracks.Select(tracks => new SelectListItem
        //    {
        //        Value = tracks.id.ToString(),
        //        Text = $"{tracks.name} - {tracks.artists.name}"
        //    });
        //    ViewData["tracksId"] = new SelectList(tracksPlaylists, "Value", "Text");

        //    var playlists = _context.Playlists.ToList();
        //    var playlistsTracks = playlists.Select(playlists => new SelectListItem
        //    {
        //        Value = playlists.id.ToString(),
        //        //username = playlists.username,
        //        Text = $"{playlists.name} - {playlists.username}"
        //    });//.Where(p => p.username == User.Identity.Name)
        //       //.ToListAsync();
        //    ViewData["playlistsId"] = new SelectList(playlistsTracks, "Value", "Text");
        //    return View();
        //    //ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name");
        //    //ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name");
        //    //return View();
        //}

        //// POST: TracksPlaylists/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,tracksId,playlistsId")] TracksPlaylists tracksPlaylists)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(tracksPlaylists);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    /*var playlists = await _context.Playlists
        //        .Where(p => p.username == User.Identity.Name)
        //        .ToListAsync();
        //    ViewData["playlistsId"] = new SelectList(playlists, "id", "name");*/
        //    ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name", tracksPlaylists.playlistsId);
        //    ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name", tracksPlaylists.tracksId);
        //    return View(tracksPlaylists);
        //}

        // GET: TracksPlaylists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TracksPlaylists == null)
            {
                return NotFound();
            }

            var tracksPlaylists = await _context.TracksPlaylists.FindAsync(id);
            if (tracksPlaylists == null)
            {
                return NotFound();
            }

            var tracks = _context.Tracks.Include(t => t.artists).ToList();
            var trackPlaylists = tracks.Select(tracks => new SelectListItem
            {
                Value = tracks.id.ToString(),
                Text = $"{tracks.name} - {tracks.artists.name}"
            });
            ViewData["tracksId"] = new SelectList(trackPlaylists, "Value", "Text");

            var playlists = _context.Playlists.ToList();
            var playlistsTracks = playlists.Select(playlists => new SelectListItem
            {
                Value = playlists.id.ToString(),
                Text = $"{playlists.name} - {playlists.username}"
            });
            ViewData["playlistsId"] = new SelectList(playlistsTracks, "Value", "Text");

            //ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name", tracksPlaylists.playlistsId);
            //ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name", tracksPlaylists.tracksId);
            return View(tracksPlaylists);
        }

        // POST: TracksPlaylists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,tracksId,playlistsId")] TracksPlaylists tracksPlaylists)
        {
            if (id != tracksPlaylists.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tracksPlaylists);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TracksPlaylistsExists(tracksPlaylists.Id))
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
            ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name", tracksPlaylists.playlistsId);
            ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name", tracksPlaylists.tracksId);
            return View(tracksPlaylists);
        }

        // GET: TracksPlaylists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TracksPlaylists == null)
            {
                return NotFound();
            }

            var tracksPlaylists = await _context.TracksPlaylists
                .Include(t => t.playlists)
                .Include(t => t.tracks)
                .ThenInclude(t => t.artists)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracksPlaylists == null)
            {
                return NotFound();
            }

            return View(tracksPlaylists);
        }

        // POST: TracksPlaylists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TracksPlaylists == null)
            {
                return Problem("Entity set 'DataContext.TracksPlaylists'  is null.");
            }
            var tracksPlaylists = await _context.TracksPlaylists.FindAsync(id);
            if (tracksPlaylists != null)
            {
                _context.TracksPlaylists.Remove(tracksPlaylists);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TracksPlaylistsExists(int id)
        {
          return (_context.TracksPlaylists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

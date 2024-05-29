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

namespace CourseWork.Controllers
{
    [Authorize(Roles = "user")]
    public class TracksPlaylistsController1 : Controller
    {
        private readonly DataContext _context;

        public TracksPlaylistsController1(DataContext context)
        {
            _context = context;
        }

        // GET: TracksPlaylists
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.TracksPlaylists.Include(t => t.playlists);
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
                .Include(t => t.playlists)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracksPlaylists == null)
            {
                return NotFound();
            }

            return View(tracksPlaylists);
        }

        // GET: TracksPlaylists/Create
        public IActionResult Create()
        {
            /*var tracks = _context.Tracks.ToList();
            var tracksPlaylists = tracks.Select(tracks => new SelectListItem
            {
                Value = tracks.id.ToString(),
                Text = tracks.name
            });
            ViewData["tracksId"] = new SelectList(tracksPlaylists, "Value", "Text");*/
            ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name");

            var playlists = _context.Playlists.ToList();
            var playlistsTracks = playlists.Select(playlists => new SelectListItem
            {
                Value = playlists.id.ToString(),
                Text = $"{playlists.name} - {playlists.username}"
            });
            ViewData["playlistsId"] = new SelectList(playlistsTracks, "Value", "Text");
            //ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name");
            return View();
        }

        // POST: TracksPlaylists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,tracksId,playlistsId")] TracksPlaylists tracksPlaylists)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tracksPlaylists);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name", tracksPlaylists.playlistsId);
            ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name", tracksPlaylists.tracksId);
            return View(tracksPlaylists);
        }

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

            ViewData["tracksId"] = new SelectList(_context.Tracks, "id", "name");


            var playlists = _context.Playlists.ToList();
            var playlistTrack = playlists.Select(playlists => new SelectListItem
            {
                Value = playlists.id.ToString(),
                Text = $"{playlists.name} - {playlists.username}"
            });
            ViewData["playlistsId"] = new SelectList(playlistTrack, "Value", "Text");

            //ViewData["playlistsId"] = new SelectList(_context.Playlists, "id", "name", tracksPlaylists.playlistsId);
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

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
using Microsoft.AspNetCore.Identity;
using CourseWork.Areas.Identity.Data;

namespace CourseWork.Controllers
{
    [Authorize(Roles = "user, moderator")]
    public class CommentsController : Controller
    {
        private readonly UserManager<CourseWorkUser> _userManager;
        private readonly DataContext _context;

        public CommentsController(DataContext context, UserManager<CourseWorkUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Comments.Include(c => c.albums);
            return View(await dataContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Include(c => c.albums)
                .FirstOrDefaultAsync(m => m.id == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // GET: Comments/Create
        public IActionResult Create(int id)
        {
            var currentId = id;
            var comments = _context.Comments.Where(c => c.id == id).ToList();
            var album = comments.Select(comment => new SelectListItem
            {
                Value = comment.id.ToString()
            }).ToList();
            ViewData["albumsId"] = new SelectList(album, "Value", "Value");
            ViewData["albumsId"] = new SelectList(_context.Albums, "id", "name");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,description,albumsId")] Comments comments)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                comments.username = await _userManager.GetUserNameAsync(user);
                comments.date = DateTime.Now;
                _context.Add(comments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["albumsId"] = new SelectList(_context.Albums, "id", "name", comments.albumsId);
            return View(comments);
        }
        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments.FindAsync(id);
            if (comments == null)
            {
                return NotFound();
            }
            ViewData["albumsId"] = new SelectList(_context.Albums, "id", "name", comments.albumsId);
            return View(comments);
        }
        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,username,description,date,albumsId")] Comments comments)
        {
            if (id != comments.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentsExists(comments.id))
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
            ViewData["albumsId"] = new SelectList(_context.Albums, "id", "name", comments.albumsId);
            return View(comments);
        }
        [Authorize(Roles = "moderator")]
        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Include(c => c.albums)
                .FirstOrDefaultAsync(m => m.id == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }
        [Authorize(Roles = "moderator")]
        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'DataContext.Comments'  is null.");
            }
            var comments = await _context.Comments.FindAsync(id);
            if (comments != null)
            {
                _context.Comments.Remove(comments);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentsExists(int id)
        {
          return (_context.Comments?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}

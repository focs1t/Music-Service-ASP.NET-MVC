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
using OfficeOpenXml;

namespace CourseWork.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class ConcertsController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public ConcertsController(DataContext context, IWebHostEnvironment appEvironment)
        {
            _context = context;
            _appEnvironment = appEvironment;
        }

        // GET: Concerts
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Concerts.Include(c => c.tours);
            return View(await dataContext.ToListAsync());
        }

        // GET: Concerts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Concerts == null)
            {
                return NotFound();
            }

            var concerts = await _context.Concerts
                .Include(c => c.tours)
                .FirstOrDefaultAsync(m => m.id == id);
            if (concerts == null)
            {
                return NotFound();
            }

            return View(concerts);
        }
        [Authorize(Roles = "admin")]
        // GET: Concerts/Create
        public IActionResult Create()
        {
            ViewData["toursId"] = new SelectList(_context.Tours, "id", "name");
            return View();
        }
        [Authorize(Roles = "admin")]
        // POST: Concerts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,city,date,toursId")] Concerts concerts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(concerts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["toursId"] = new SelectList(_context.Tours, "id", "name", concerts.toursId);
            return View(concerts);
        }
        [Authorize(Roles = "admin")]
        // GET: Concerts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Concerts == null)
            {
                return NotFound();
            }

            var concerts = await _context.Concerts.FindAsync(id);
            if (concerts == null)
            {
                return NotFound();
            }
            ViewData["toursId"] = new SelectList(_context.Tours, "id", "name", concerts.toursId);
            return View(concerts);
        }
        [Authorize(Roles = "admin")]
        // POST: Concerts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,city,date,toursId")] Concerts concerts)
        {
            if (id != concerts.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(concerts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConcertsExists(concerts.id))
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
            ViewData["toursId"] = new SelectList(_context.Tours, "id", "name", concerts.toursId);
            return View(concerts);
        }
        [Authorize(Roles = "admin")]
        // GET: Concerts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Concerts == null)
            {
                return NotFound();
            }

            var concerts = await _context.Concerts
                .Include(c => c.tours)
                .FirstOrDefaultAsync(m => m.id == id);
            if (concerts == null)
            {
                return NotFound();
            }

            return View(concerts);
        }
        [Authorize(Roles = "admin")]
        // POST: Concerts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Concerts == null)
            {
                return Problem("Entity set 'DataContext.Concerts'  is null.");
            }
            var concerts = await _context.Concerts.FindAsync(id);
            if (concerts != null)
            {
                _context.Concerts.Remove(concerts);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConcertsExists(int id)
        {
          return (_context.Concerts?.Any(e => e.id == id)).GetValueOrDefault();
        }

        public FileResult GetReport()
        {
            // Путь к файлу с шаблоном
            string path = "/reports/concerts.xlsx";
            //Путь к файлу с результатом
            string result = "/reports/concerts/concerts.xlsx";
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + result);
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            int sum = 0;
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = $"{User.Identity.Name}";
                excelPackage.Workbook.Properties.Title = "Список концертов";
                excelPackage.Workbook.Properties.Subject = "Концерты";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet =
               excelPackage.Workbook.Worksheets["concerts"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 1;
                Dictionary<Artists, int> artists = new Dictionary<Artists, int>();
                Dictionary<Tours, int> tours = new Dictionary<Tours, int>();
                List<Concerts> concerts = _context.Concerts.ToList();
                foreach (Concerts crt in concerts)
                {
                    Tours tr = _context.Tours.Find(crt.toursId);
                    if (!tours.ContainsKey(tr))
                    {
                        tours.Add(tr, 1);
                    }
                }
                List<Tours> distTr = new List<Tours>(tours.Keys.ToList());
                foreach (Tours tr in distTr)
                {
                    Artists art = _context.Artists.Find(tr.artistsId);
                    if (!artists.ContainsKey(art))
                    {
                        artists.Add(art, 1);
                    }
                }
                List<Artists> distArt = new List<Artists>(artists.Keys.ToList());
                foreach (Artists art in distArt)
                {
                    worksheet.Cells[startLine, 1].Value = "Исполнитель";
                    worksheet.Cells[startLine++, 2].Value = art.name;
                    List<Tours> toursOfArtist = new List<Tours>();
                    foreach (Tours tr in distTr)
                    {
                        if(tr.artistsId == art.id)
                        {
                            toursOfArtist.Add(tr);
                        }
                    }
                    foreach (Tours tr in toursOfArtist)
                    {
                        List<Concerts> concertsOfTour = new List<Concerts>();
                        worksheet.Cells[startLine, 1].Value = "Название тура";
                        worksheet.Cells[startLine++, 2].Value = tr.name;
                        foreach (Concerts crt in concerts)
                        {
                            if (crt.toursId == tr.id)
                            {
                                worksheet.Cells[startLine, 1].Value = "Название концерта";
                                worksheet.Cells[startLine, 2].Value = crt.name;
                                worksheet.Cells[startLine, 3].Value = "Дата";
                                worksheet.Cells[startLine, 4].Value = crt.date.ToString("yyyy-MM-dd HH:mm");
                                worksheet.Cells[startLine, 5].Value = "Город";
                                worksheet.Cells[startLine++, 6].Value = crt.city;
                                concertsOfTour.Add(crt);
                            }
                        }
                    }
                    startLine += 1;
                }
                // Сохраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type = "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "concerts.xlsx";
            return File(result, file_type, file_name);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using isilani.Data;
using isilani.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace isilani.Controllers
{
    public class IsIlanlariController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<IsIlanlariController> _logger;

        public IsIlanlariController(AppDbContext context, ILogger<IsIlanlariController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // DebugAll endpoint removed for delivery.

     
        // Reseed endpoint removed because DbInitializer was deleted. Seed only during development if needed.

        
        [HttpGet]
        public async Task<IActionResult> Raw(int id)
        {
            var rec = await _context.IsIlanlari.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (rec == null) return NotFound();
            return Json(rec);
        }

        
        public async Task<IActionResult> Index(string arama, int? kategoriId, string siralama)
        {
            ViewData["CurrentSearch"] = arama;
            ViewData["CurrentCategory"] = kategoriId;
            ViewData["CurrentSort"] = siralama;

            var q = _context.IsIlanlari.Include(j => j.IsKategori).AsQueryable();

            if (!string.IsNullOrEmpty(arama))
                q = q.Where(j => j.Baslik.Contains(arama));

            if (kategoriId.HasValue && kategoriId.Value > 0)
                q = q.Where(j => j.IsKategoriId == kategoriId.Value);

            q = (siralama ?? string.Empty) switch
            {
                "baslik_asc" => q.OrderBy(j => j.Baslik),
                "baslik_desc" => q.OrderByDescending(j => j.Baslik),
                "maas_desc" => q.OrderByDescending(j => j.Maas),
                "maas_asc" => q.OrderBy(j => j.Maas),
                "sirket_asc" => q.OrderBy(j => j.Sirket),
                "sirket_desc" => q.OrderByDescending(j => j.Sirket),
                "konum_asc" => q.OrderBy(j => j.Konum),
                "konum_desc" => q.OrderByDescending(j => j.Konum),
                _ => q.OrderBy(j => j.Baslik)
            };

            ViewBag.Kategoriler = new SelectList(await _context.IsKategoriler.ToListAsync(), "Id", "Ad"); 

            var list = await q.ToListAsync();
            if (!list.Any()) ViewBag.Message = "Kayıt bulunamadı.";
            return View(list);
        }

       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var isIlani = await _context.IsIlanlari
                .Include(j => j.IsKategori)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (isIlani == null) return NotFound();

            return View(isIlani);
        }

       
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Kategoriler = new SelectList(_context.IsKategoriler.ToList(), "Id", "Ad");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Baslik,Sirket,Konum,Maas,Aciklama,IsKategoriId")] IsIlani isIlani)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Creating IsIlani: {Title}", isIlani.Baslik);
                    _context.Add(isIlani);
                    var affected = await _context.SaveChangesAsync();
                    _logger.LogInformation("Create saved, affected rows: {Count}", affected);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving new IsIlani");
                    ModelState.AddModelError(string.Empty, "Kayıt sırasında hata oluştu.");
                }
            }

           
            foreach (var entry in ModelState)
            {
                if (entry.Value.Errors.Any())
                {
                    _logger.LogWarning("ModelState error for {Key}: {Errors}", entry.Key, string.Join(" | ", entry.Value.Errors.Select(e => e.ErrorMessage)));
                }
            }
           
            ViewBag.ModelErrors = ModelState.Where(m => m.Value.Errors.Count > 0)
                .SelectMany(m => m.Value.Errors.Select(err => new KeyValuePair<string,string>(m.Key, err.ErrorMessage))).ToList();
            ViewBag.Kategoriler = new SelectList(_context.IsKategoriler.ToList(), "Id", "Ad", isIlani.IsKategoriId);
            return View(isIlani);
        }

      
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var isIlani = await _context.IsIlanlari.FindAsync(id);
            if (isIlani == null) return NotFound();
            ViewBag.Kategoriler = new SelectList(_context.IsKategoriler.ToList(), "Id", "Ad", isIlani.IsKategoriId);
            return View(isIlani);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Baslik,Sirket,Konum,Maas,Aciklama,IsKategoriId")] IsIlani isIlani)
        {
            if (id != isIlani.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Updating IsIlani id={Id}", isIlani.Id);
                    var existing = await _context.IsIlanlari.FindAsync(id);
                    if (existing == null) return NotFound();
                   
                    existing.Baslik = isIlani.Baslik;
                    existing.Sirket = isIlani.Sirket;
                    existing.Konum = isIlani.Konum;
                    existing.Maas = isIlani.Maas;
                    existing.Aciklama = isIlani.Aciklama;
                    existing.IsKategoriId = isIlani.IsKategoriId;

                    var affected = await _context.SaveChangesAsync();
                    _logger.LogInformation("Update saved, affected rows: {Count}", affected);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IsIlaniExists(isIlani.Id)) return NotFound();
                    else throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating IsIlani id={Id}", isIlani.Id);
                    ModelState.AddModelError(string.Empty, "Güncelleme sırasında hata oluştu.");
                }
            }
           
            try { TempData["PostedModel"] = System.Text.Json.JsonSerializer.Serialize(isIlani); } catch { }
           
            var errors = new List<string>();
            foreach (var entry in ModelState)
            {
                if (entry.Value.Errors.Any())
                {
                    var msgs = entry.Value.Errors.Select(e => e.ErrorMessage).ToArray();
                    _logger.LogWarning("ModelState error for {Key}: {Errors}", entry.Key, string.Join(" | ", msgs));
                    foreach (var m in msgs) errors.Add($"{entry.Key}: {m}");
                }
            }
            TempData["ModelErrors"] = string.Join("\n", errors);
            ViewBag.ModelErrors = errors.Select(s => new KeyValuePair<string,string>("", s)).ToList();

            ViewBag.Kategoriler = new SelectList(_context.IsKategoriler.ToList(), "Id", "Ad", isIlani.IsKategoriId);
            return View(isIlani);
        }

       
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var isIlani = await _context.IsIlanlari
                .Include(j => j.IsKategori)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (isIlani == null) return NotFound();

            return View(isIlani);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isIlani = await _context.IsIlanlari.FindAsync(id);
            if (isIlani != null)
            {
                _context.IsIlanlari.Remove(isIlani);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IsIlaniExists(int id)
        {
            return _context.IsIlanlari.Any(e => e.Id == id);
        }
    }
}

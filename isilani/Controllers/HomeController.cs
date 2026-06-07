using isilani.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using isilani.Data;
using Microsoft.EntityFrameworkCore;

namespace isilani.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ToplamIlan = await _context.IsIlanlari.CountAsync();
            ViewBag.ToplamKategori = await _context.IsKategoriler.CountAsync();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

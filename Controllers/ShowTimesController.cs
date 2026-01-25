using CinemaBooking.Data;
using CinemaBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    public class ShowTimesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShowTimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var showtimes = _context.ShowTimes
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .OrderBy(s => s.TimeSlot)
                .ToList();

            return View(showtimes);
        }

        public IActionResult Create()
        {
            ViewBag.MovieId = new SelectList(_context.Movies, "Id", "Title");
            ViewBag.CinemaId = new SelectList(_context.Cinemas, "Id", "Name");
            return View();
        }




        [HttpPost]
        public IActionResult Create(ShowTime showTime)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                ViewBag.MovieId = new SelectList(_context.Movies, "Id", "Title");
                ViewBag.CinemaId = new SelectList(_context.Cinemas, "Id", "Name");
                return View(showTime);
            }

            _context.ShowTimes.Add(showTime);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        public IActionResult Edit(int id)
        {
            var showTime = _context.ShowTimes.Find(id);
            if (showTime == null) return NotFound();

            ViewBag.MovieId = new SelectList(_context.Movies, "Id", "Title", showTime.MovieId);
            return View(showTime);
        }

        [HttpPost]
        public IActionResult Edit(ShowTime showTime)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MovieId = new SelectList(_context.Movies, "Id", "Title", showTime.MovieId);
                return View(showTime);
            }

            _context.ShowTimes.Update(showTime);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var showTime = _context.ShowTimes
                .Include(s => s.Movie)
                .FirstOrDefault(s => s.Id == id);

            if (showTime == null) return NotFound();
            return View(showTime);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var showTime = _context.ShowTimes.Find(id);
            if (showTime == null) return NotFound();

            _context.ShowTimes.Remove(showTime);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

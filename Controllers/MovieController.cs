using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Movie movie)
        {
            if (!ModelState.IsValid)
                return View(movie);

            _context.Movies.Add(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null) return NotFound();
            return View(movie);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Movie movie)
        {
            if (!ModelState.IsValid)
                return View(movie);

            _context.Movies.Update(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Details(int id)
        {
            var movie = _context.Movies
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return NotFound();

            var showTimes = _context.ShowTimes
                .Include(s => s.Cinema)   
                .Where(s => s.MovieId == id)
                .OrderBy(s => s.TimeSlot)
                .ToList();

            ViewBag.Dates = showTimes
                .Select(s => s.TimeSlot.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            var vm = new MovieDetailsViewModel
            {
                Movie = movie,
                ShowTimes = showTimes
            };

            return View(vm);
        }

    }
}
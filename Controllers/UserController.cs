using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Kiểm tra người dùng đã đăng nhập
        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        private User? GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return null;

            return _context.users.FirstOrDefault(u => u.Id == userId);
        }

        // GET: User/Index (Trang quản lý thông tin cá nhân)
        [HttpGet]
        public IActionResult Index()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }

        // GET: User/Edit (Chỉnh sửa thông tin)
        [HttpGet]
        public IActionResult Edit()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }

        // POST: User/Edit (Lưu chỉnh sửa thông tin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = _context.users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra email không bị trùng với người dùng khác
            if (user.Email != model.Email && _context.users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng!");
                return View(user);
            }

            // Cập nhật thông tin
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber ?? string.Empty;
            user.UpdatedAt = DateTime.Now;

            try
            {
                _context.users.Update(user);
                _context.SaveChanges();

                // Cập nhật session
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserFullName", user.FullName);

                _logger.LogInformation($"User {user.Email} updated profile.");
                TempData["SuccessMessage"] = "Thông tin đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                ModelState.AddModelError("", "Có lỗi khi cập nhật thông tin!");
                return View(user);
            }
        }

        // GET: User/BookingHistory (Lịch sử đặt vé)
        [HttpGet]
        public IActionResult BookingHistory()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var bookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Cinema)
                .Include(b => b.Seat)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            return View(bookings);
        }

        // GET: User/BookingDetail (Chi tiết đơn đặt vé)
        [HttpGet]
        public IActionResult BookingDetail(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var booking = _context.Bookings
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Cinema)
                .Include(b => b.Seat)
                .FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: User/Dashboard (Trang tổng quan người dùng)
        [HttpGet]
        public IActionResult Dashboard()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = GetCurrentUser();
            if (user == null)
            {
                return NotFound();
            }

            // Thống kê
            var totalBookings = _context.Bookings.Where(b => b.UserId == userId).Count();
            var upcomingBookings = _context.Bookings
                .Where(b => b.UserId == userId && b.ShowTime.StartTime > DateTime.Now)
                .Count();

            var pastBookings = _context.Bookings
                .Where(b => b.UserId == userId && b.ShowTime.StartTime <= DateTime.Now)
                .Count();

            var recentBookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Movie)
                .Include(b => b.ShowTime)
                    .ThenInclude(st => st.Cinema)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToList();

            ViewBag.User = user;
            ViewBag.TotalBookings = totalBookings;
            ViewBag.UpcomingBookings = upcomingBookings;
            ViewBag.PastBookings = pastBookings;
            ViewBag.RecentBookings = recentBookings;

            return View();
        }

        // POST: User/CancelBooking (Hủy đặt vé)
        [HttpPost]
        public IActionResult CancelBooking(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id && b.UserId == userId);
            if (booking == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu suất chiếu chưa bắt đầu mới có thể hủy
            if (booking.ShowTime.StartTime <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Không thể hủy vé sau khi suất chiếu bắt đầu!";
                return RedirectToAction(nameof(BookingHistory));
            }

            try
            {
                // Xóa booking
                _context.Bookings.Remove(booking);
                _context.SaveChanges();

                _logger.LogInformation($"User {userId} cancelled booking {id}.");
                TempData["SuccessMessage"] = "Vé đã được hủy thành công!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking");
                TempData["ErrorMessage"] = "Có lỗi khi hủy vé!";
            }

            return RedirectToAction(nameof(BookingHistory));
        }
    }
}

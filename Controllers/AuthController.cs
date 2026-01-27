using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CinemaBooking.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra email đã tồn tại
            if (_context.users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký!");
                return View(model);
            }

            // Kiểm tra mật khẩu khớp
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu không khớp!");
                return View(model);
            }

            // Tạo user mới
            var user = new Models.User
            {
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber ?? string.Empty,
                PasswordHash = Models.User.HashPassword(model.Password),
                CreatedAt = DateTime.Now
            };

            try
            {
                _context.users.Add(user);
                _context.SaveChanges();

                _logger.LogInformation($"User {user.Email} registered successfully.");
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ModelState.AddModelError("", "Có lỗi khi đăng ký. Vui lòng thử lại!");
                return View(model);
            }
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm user theo email
            var user = _context.users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null || !user.VerifyPassword(model.Password))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng!");
                return View(model);
            }

            // Lưu thông tin user vào session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserFullName", user.FullName);

            _logger.LogInformation($"User {user.Email} logged in successfully.");

            // Redirect về trang trước hoặc home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        // POST: Auth/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra mật khẩu cũ
            if (!user.VerifyPassword(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng!");
                return View(model);
            }

            // Kiểm tra mật khẩu mới khớp
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu mới không khớp!");
                return View(model);
            }

            // Cập nhật mật khẩu
            user.PasswordHash = Models.User.HashPassword(model.NewPassword);
            user.UpdatedAt = DateTime.Now;
            _context.users.Update(user);
            _context.SaveChanges();

            _logger.LogInformation($"User {user.Email} changed password.");
            TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
            return RedirectToAction("Index", "User");
        }

        // Kiểm tra session
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue;
        }
    }
}

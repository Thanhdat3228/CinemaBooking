using CinemaBooking.Data;
using CinemaBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    [AuthorizeSession]  // Bảo vệ tất cả action trong controller
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Nhận DbContext từ Dependency Injection
        // DbContext dùng để truy vấn và lưu dữ liệu vào database
        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public IActionResult SelectSeats(int showTimeId)
        {
            // Lấy thông tin ShowTime từ database
            // Include() dùng để lấy kèm Movie, Cinema, và Seats liên quan
            var showTime = _context.ShowTimes
                .Include(s => s.Movie)      // Lấy tên phim
                .Include(s => s.Cinema)     // Lấy tên rạp
                .Include(s => s.Seats)      // Lấy danh sách ghế
                .FirstOrDefault(s => s.Id == showTimeId);

            // Kiểm tra nếu ShowTime không tồn tại
            if (showTime == null)
                return NotFound("Không tìm thấy suất chiếu");

            // Lấy danh sách ID các ghế đã được đặt
            var bookedSeatIds = _context.Bookings
                .Where(b => b.ShowTimeId == showTimeId)
                .Select(b => b.SeatId)
                .ToList();

            // Gửi dữ liệu tới View thông qua ViewBag
            ViewBag.ShowTime = showTime;
            ViewBag.Seats = showTime.Seats.ToList();
            ViewBag.BookedSeatIds = bookedSeatIds;

            return View();
        }

        // ========== PHƯƠNG THỨC 2: TẠO BOOKING (ĐẶT VÉ) ==========
       
        [HttpPost]  // Phương thức này nhận dữ liệu từ form POST
        public IActionResult Confirm(int showTimeId, int[] seatIds)
        {
            // Kiểm tra người dùng đã đăng nhập
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để đặt vé";
                return RedirectToAction("Login", "Auth");
            }

            // Bước 1: Kiểm tra người dùng có chọn ghế không
            if (seatIds == null || seatIds.Length == 0)
            {
                // Nếu không chọn → lưu thông báo lỗi vào TempData
                // TempData dùng để lưu data tạm thời cho lần request tiếp theo
                TempData["Error"] = "Chọn ít nhất 1 ghế";
                return RedirectToAction("SelectSeats", new { showTimeId });
            }

            // Bước 2: Kiểm tra các ghế chọn có bị đặt rồi không
            // Nếu có ghế nào đã có booking → báo lỗi
            // FirstOrDefault() trả về phần tử đầu tiên nếu có, NULL nếu không có
            var booked = _context.Bookings
                .Where(b => b.ShowTimeId == showTimeId && seatIds.Contains(b.SeatId))
                .FirstOrDefault();

            if (booked != null)
            {
                TempData["Error"] = "Ghế này đã có người đặt";
                return RedirectToAction("SelectSeats", new { showTimeId });
            }

            // Bước 3: Tạo Booking cho từng ghế được chọn
            
            foreach (var seatId in seatIds)
            {
                // Tạo object Booking mới
                var booking = new Booking
                {
                    UserId = userId.Value,           // ID người dùng đặt vé
                    ShowTimeId = showTimeId,   // ID suất chiếu
                    SeatId = seatId,           // ID ghế được chọn
                    BookingDate = DateTime.Now // Thời gian đặt vé (ngày giờ hiện tại)
                };
                
                // Thêm Booking vào DbContext (chưa lưu vào DB lúc này)
                _context.Bookings.Add(booking);
            }

            //  Lưu tất cả Bookings vào database
            
            _context.SaveChanges();

            
            TempData["Success"] = "Đặt vé thành công!";
            
            // Chuyển hướng (Redirect) tới trang "Vé của tôi"
            return RedirectToAction("MyTickets");
        }

    
        public IActionResult MyTickets()
        {
            // Kiểm tra người dùng đã đăng nhập
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập";
                return RedirectToAction("Login", "Auth");
            }
            
            // Bước 2: Tìm tất cả Booking của user này
            // Include() dùng để load dữ liệu liên quan (ShowTime, Movie, Seat)
            // ThenInclude() dùng để load dữ liệu con (Movie nằm trong ShowTime)
            var bookings = _context.Bookings
                .Where(b => b.UserId == userId)  // Chỉ lấy booking của user này
                .Include(b => b.ShowTime)        // Lấy thông tin suất chiếu
                    .ThenInclude(s => s.Movie)   // Lấy tên phim
                .Include(b => b.Seat)            // Lấy thông tin ghế (số ghế, etc)
                .ToList();

            
            // Model của View sẽ là List<Booking>
            return View(bookings);
        }

        
        [HttpPost]  // Phương thức nhận dữ liệu POST 
        public IActionResult CancelBooking(int bookingId)
        {
          
            var booking = _context.Bookings.Find(bookingId);
            
            // Kiểm tra Booking
            if (booking == null)
                return NotFound("Không tìm thấy vé");

       
            _context.Bookings.Remove(booking);
            
            _context.SaveChanges();

            
            TempData["Success"] = "Hủy vé thành công";
            
            
            return RedirectToAction("MyTickets");
        }
    }
}
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

        // ========== PHƯƠNG THỨC 1: HIỂN THỊ SƠ ĐỒ GHẾ ==========
        /// <summary>
        /// Hiển thị sơ đồ ghế của một suất chiếu
        /// 
        /// Tham số:
        ///   - showTimeId: ID của suất chiếu
        /// 
        /// Luồng:
        /// 1. Tìm ShowTime theo ID (kèm theo Movie, Cinema, Seats)
        /// 2. Lấy danh sách ID các ghế đã bị đặt (có trong Bookings)
        /// 3. Gửi dữ liệu tới View để hiển thị sơ đồ ghế:
        ///    - Ghế trống: màu xanh (có thể chọn)
        ///    - Ghế đã đặt: màu đỏ (bị disable)
        /// </summary>
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
        /// <summary>
        /// Xử lý khi người dùng chọn ghế và bấm "Xác nhận"
        /// 
        /// Tham số:
        ///   - showTimeId: ID suất chiếu
        ///   - seatIds: Array ID các ghế được chọn
        /// 
        /// Luồng:
        /// 1. Kiểm tra người dùng chọn ít nhất 1 ghế
        /// 2. Kiểm tra các ghế chọn có trống không
        /// 3. Nếu trống → Tạo Booking cho từng ghế
        /// 4. Lưu vào database
        /// 5. Redirect tới trang xem vé
        /// </summary>
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

            // Bước 4: Lưu tất cả Bookings vào database
            // SaveChanges() thực thi tất cả các thay đổi (Add, Update, Delete)
            _context.SaveChanges();

            // Bước 5: Hiển thị thông báo thành công
            TempData["Success"] = "Đặt vé thành công!";
            
            // Chuyển hướng (Redirect) tới trang "Vé của tôi"
            return RedirectToAction("MyTickets");
        }

        // ========== PHƯƠNG THỨC 3: HIỂN THỊ VÉ CỦA TÔI ==========
        /// <summary>
        /// Hiển thị danh sách vé của người dùng hiện tại
        /// 
        /// Luồng:
        /// 1. Lấy UserId của người dùng hiện tại
        /// 2. Tìm tất cả Booking của user này
        /// 3. Include ShowTime, Movie, Seat (để hiển thị đầy đủ thông tin)
        /// 4. Truyền dữ liệu tới View
        /// </summary>
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

            // Bước 3: Truyền danh sách Bookings tới View
            // Model của View sẽ là List<Booking>
            return View(bookings);
        }

        // ========== PHƯƠNG THỨC 4: HỦY VÉ ==========
        /// <summary>
        /// Hủy vé (xóa Booking khỏi database)
        /// 
        /// Tham số:
        ///   - bookingId: ID của vé cần hủy
        /// 
        /// Luồng:
        /// 1. Tìm Booking theo ID
        /// 2. Kiểm tra nó có tồn tại không
        /// 3. Xóa Booking khỏi database (ghế trở nên trống)
        /// 4. Hiển thị thông báo thành công
        /// 5. Quay lại trang "Vé của tôi"
        /// </summary>
        [HttpPost]  // Phương thức nhận dữ liệu POST (bảo mật hơn GET)
        public IActionResult CancelBooking(int bookingId)
        {
            // Bước 1: Tìm Booking theo ID
            // Find() là cách nhanh nhất để tìm theo Primary Key (ID)
            var booking = _context.Bookings.Find(bookingId);
            
            // Kiểm tra Booking có tồn tại không
            if (booking == null)
                return NotFound("Không tìm thấy vé");

            // Bước 2: Xóa Booking khỏi DbContext
            // Khi xóa Booking, ghế sẽ trở nên trống (có thể đặt lại)
            _context.Bookings.Remove(booking);
            
            // Bước 3: Lưu thay đổi vào database
            _context.SaveChanges();

            // Bước 4: Hiển thị thông báo thành công
            TempData["Success"] = "Hủy vé thành công";
            
            // Bước 5: Chuyển hướng về trang "Vé của tôi"
            return RedirectToAction("MyTickets");
        }
    }
}
namespace CinemaBooking.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        // Mã vé duy nhất
        public string TicketCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        // Khóa ngoại tới Booking
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        // Giá vé
        public decimal Price { get; set; }

        // Ngày phát hành vé
        public DateTime IssuedDate { get; set; } = DateTime.Now;

        // Trạng thái: Active (hoạt động), Cancelled (hủy)
        public string Status { get; set; } = "Active"; // Active, Cancelled
    }
}

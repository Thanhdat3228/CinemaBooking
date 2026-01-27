namespace CinemaBooking.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; } = string.Empty; // A1, A2, B1, ...
        public string SeatType { get; set; } = "Normal"; // Normal, VIP, Couple
        public decimal Price { get; set; } = 100000; // Giá ghế
        public bool IsAvailable { get; set; } = true; // Ghế còn trống
        
        public int ShowTimeId { get; set; }
        public ShowTime? ShowTime { get; set; }
        
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
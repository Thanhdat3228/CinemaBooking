using CinemaBooking.Models;

namespace CinemaBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // Khóa ngoại tới User
        public int UserId { get; set; }
        public User User { get; set; }

        // Khóa ngoại tới ShowTime
        public int ShowTimeId { get; set; }
        public ShowTime ShowTime { get; set; }

        // Khóa ngoại tới Seat
        public int SeatId { get; set; }
        public Seat Seat { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;
    }
}
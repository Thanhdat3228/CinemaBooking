namespace CinemaBooking.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; } // A1, A2, B1, ...
        public int ShowTimeId { get; set; }
        public ShowTime ShowTime { get; set; }
    }
}
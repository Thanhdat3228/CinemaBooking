using System;

namespace CinemaBooking.Models
{
    public class ShowTime
    {
        public int Id { get; set; }

        // Khóa ngoại tới Movie
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // Khóa ngoại tới Cinema
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }

        // Thời gian chiếu
        public DateTime TimeSlot { get; set; }

        // Navigation property: nhiều Booking
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
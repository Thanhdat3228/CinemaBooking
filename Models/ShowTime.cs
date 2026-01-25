using System;

namespace CinemaBooking.Models
{
    public class ShowTime
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        
        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }

        public DateTime TimeSlot { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
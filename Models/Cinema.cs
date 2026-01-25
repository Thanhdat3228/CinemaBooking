namespace CinemaBooking.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ShowTime> ShowTimes { get; set; }
    }
    }

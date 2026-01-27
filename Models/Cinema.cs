namespace CinemaBooking.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        
        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }
}
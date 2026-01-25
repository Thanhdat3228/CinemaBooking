namespace CinemaBooking.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public string? Genre { get; set; }

        public int DurationMinutes { get; set; }

        public string? Rating { get; set; }

        public DateTime ReleaseDate { get; set; }

        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }
}

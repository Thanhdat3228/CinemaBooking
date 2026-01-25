namespace CinemaBooking.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; } = null!;
        public List<ShowTime> ShowTimes { get; set; } = new();
    }

}

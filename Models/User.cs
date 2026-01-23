namespace CinemaBooking.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Có thể thêm các thông tin khác
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Navigation property: nhiều Booking
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
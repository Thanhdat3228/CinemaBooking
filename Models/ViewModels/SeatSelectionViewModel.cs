using CinemaBooking.Models;

namespace CinemaBooking.Models.ViewModels
{
    public class SeatSelectionViewModel
    {
        public int ShowTimeId { get; set; }
        public int SelectedSeatId { get; set; }
        public int UserId { get; set; }

        public string MovieTitle { get; set; } = string.Empty;
        public string CinemaName { get; set; } = string.Empty;
        public DateTime TimeSlot { get; set; }
        public decimal Price { get; set; }

        // Danh sách ghế
        public List<SeatDTO> Seats { get; set; } = new List<SeatDTO>();
    }

    public class SeatDTO
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
}


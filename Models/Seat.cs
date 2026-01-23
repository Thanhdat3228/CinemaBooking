namespace CinemaBooking.Models
{
        public class Seat
        {
            public int Id { get; set; }

            // Số ghế (ví dụ: A1, B5)
            public string SeatNumber { get; set; } = string.Empty;

            // Khóa ngoại tới Cinema hoặc Room
            public int CinemaId { get; set; }
            public Cinema Cinema { get; set; }

            // Có thể thêm trạng thái ghế
            public bool IsAvailable { get; set; } = true;
        }

}

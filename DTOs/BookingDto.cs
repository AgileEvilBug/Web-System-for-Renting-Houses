namespace RentifyApi.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int? UserProfileId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
    }
}

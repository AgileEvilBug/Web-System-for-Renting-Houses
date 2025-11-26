namespace RentifyApi.DTOs
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Address { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public string? ImageUrl { get; set; }
    }
}

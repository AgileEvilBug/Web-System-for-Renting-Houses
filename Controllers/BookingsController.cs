using Microsoft.AspNetCore.Mvc;
using RentifyApi.Data;
using RentifyApi.DTOs;
using RentifyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace RentifyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BookingsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Bookings.Include(b => b.Property).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(BookingDto dto)
        {
            // Basic validation: check property exists and date validity
            var prop = await _db.Properties.FindAsync(dto.PropertyId);
            if (prop == null) return BadRequest("Property not found");
            if (dto.EndDate <= dto.StartDate) return BadRequest("EndDate must be after StartDate");

            var days = (dto.EndDate - dto.StartDate).Days;
            var total = prop.PricePerNight * Math.Max(days, 1);

            var booking = new Booking {
                PropertyId = dto.PropertyId,
                UserProfileId = dto.UserProfileId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalPrice = total,
                Status = "Pending"
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = booking.Id }, booking);
        }
    }
}

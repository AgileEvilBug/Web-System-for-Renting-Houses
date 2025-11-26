using Microsoft.EntityFrameworkCore;
using RentifyApi.Data;
using RentifyApi.Models;

namespace RentifyApi.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly AppDbContext _db;
        public PropertyRepository(AppDbContext db) => _db = db;

        public async Task<List<Property>> GetAllAsync() =>
            await _db.Properties.AsNoTracking().ToListAsync();

        public async Task<Property?> GetByIdAsync(int id) =>
            await _db.Properties.Include(p => p.Bookings).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Property> AddAsync(Property property)
        {
            _db.Properties.Add(property);
            await _db.SaveChangesAsync();
            return property;
        }

        public async Task UpdateAsync(Property property)
        {
            _db.Properties.Update(property);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var p = await _db.Properties.FindAsync(id);
            if (p != null)
            {
                _db.Properties.Remove(p);
                await _db.SaveChangesAsync();
            }
        }
    }
}

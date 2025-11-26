using Microsoft.EntityFrameworkCore;
using RentifyApi.Models;

namespace RentifyApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<UserProfile> Users => Set<UserProfile>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>().HasKey(u => u.Id);
            modelBuilder.Entity<Property>().HasKey(p => p.Id);
            modelBuilder.Entity<Booking>().HasKey(b => b.Id);

            modelBuilder.Entity<Property>()
                .HasMany(p => p.Bookings)
                .WithOne(b => b.Property!)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

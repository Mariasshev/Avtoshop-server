using Data_Access.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;


namespace Data_Access.Context
{
    public class CarContext : DbContext
    {
        public DbSet<CarPhoto> CarPhotos { get; set; }

        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Saler> Salers { get; set; }

        public DbSet<CarBrands> Brands { get; set; } = null!;

        public DbSet<CarModel> Models { get; set; } = null!;

        public DbSet<BlogItem> Blog { get; set; }

        public CarContext(DbContextOptions<CarContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {

            mb.Entity<Order>()
                .HasOne(o => o.Car)
                .WithOne(c => c.Order)
                .HasForeignKey<Order>(o => o.CarId)
                .OnDelete(DeleteBehavior.Restrict); // или SetNull, по твоей 
        }


    }
}

using Microsoft.EntityFrameworkCore;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Data
{
    public class OnlineStoreDbContext : DbContext
    {
        public OnlineStoreDbContext(DbContextOptions<OnlineStoreDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product Hierarchy (TPH)
            modelBuilder.Entity<Product>()
                .HasDiscriminator<string>("ProductType")
                .HasValue<ElectronicProduct>("Electronic")
                .HasValue<ClothingProduct>("Clothing")
                .HasValue<VehicleProduct>("Vehicle");

            // Configure User Hierarchy (TPH)
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Customer>("Customer")
                .HasValue<Admin>("Admin");

            // Relationships

            // Order - Products (Many-to-Many)
            // Using a join table "OrderProducts"
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Products)
                .WithMany()
                .UsingEntity("OrderProducts");

            // Order - User (One-to-Many relationship direction)
            // Since User is base, we map to User.
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany();

            // Configure decimal precision for Price/Total
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasColumnType("decimal(18,2)");
        }
    }
}

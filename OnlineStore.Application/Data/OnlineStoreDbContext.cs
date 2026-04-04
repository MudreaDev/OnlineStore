using Microsoft.EntityFrameworkCore;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using System.Text.Json;
using System.Collections.Generic;

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
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<SubCategory> SubCategories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ProductImages
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasOne(pi => pi.Product)
                    .WithMany(p => p.Images)
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(pi => pi.ImageUrl).IsRequired();
                entity.Property(pi => pi.PublicId).IsRequired();
            });

            // Configure Product Hierarchy (TPH)
            modelBuilder.Entity<Product>()
                .HasDiscriminator<string>("ProductType")
                .HasValue<ElectronicProduct>("Electronic")
                .HasValue<ClothingProduct>("Clothing")
                .HasValue<VehicleProduct>("Vehicle")
                .HasValue<DynamicProduct>("Dynamic");

            // Configure DynamicProduct JSON storage
            modelBuilder.Entity<DynamicProduct>()
                .Property(dp => dp.CustomAttributes)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
                );

            // Configure User Hierarchy (TPH)
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Customer>("Customer")
                .HasValue<Admin>("Admin");

            // Relationships

            // Order - OrderItems (One-to-Many)
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne<Order>()
                    .WithMany(o => o.Items)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(oi => oi.ProductName).IsRequired();
            });

            // Order - User (One-to-Many relationship direction)
            // Since User is base, we map to User.
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany();



            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Configure SubscriberEmails to be stored as a comma-separated string
            modelBuilder.Entity<Product>()
                .Property(p => p.SubscriberEmails)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasColumnType("decimal(18,2)");
        }
    }
}

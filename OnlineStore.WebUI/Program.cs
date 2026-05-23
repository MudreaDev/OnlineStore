using OnlineStore.Application.Data;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Factories;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Application.Services;
using OnlineStore.Domain.Interfaces;
using OnlineStore.Application.Patterns.Mediator;
using OnlineStore.Domain.Strategies;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Enable Session
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<CurrencyRateProvider>();

// Register DbContext
builder.Services.AddDbContext<OnlineStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories as Scoped (Db storage)
builder.Services.AddScoped<DbProductRepository>();
builder.Services.AddScoped<IReadableRepository<Product>>(sp => sp.GetRequiredService<DbProductRepository>());
builder.Services.AddScoped<IWriteableRepository<Product>>(sp => sp.GetRequiredService<DbProductRepository>());

builder.Services.AddScoped<DbUserRepository>();
builder.Services.AddScoped<DbOrderRepository>();
builder.Services.AddScoped<IReadableRepository<Order>>(sp => sp.GetRequiredService<DbOrderRepository>());
builder.Services.AddScoped<IWriteableRepository<Order>>(sp => sp.GetRequiredService<DbOrderRepository>());

// Register Services
builder.Services.AddScoped<ProductAvailabilityService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<OrderNotificationService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IPaymentProcessor, LocalPaymentProcessor>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<OrderValidationService>();
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddScoped<ReviewNotificationService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<ToastService>();

// Strategy Pattern for OrderService
builder.Services.AddScoped<IDiscountStrategy, NoDiscountStrategy>();
builder.Services.AddScoped<OrderService>();

// Mediator Pattern
builder.Services.AddScoped<ICheckoutMediator, OnlineStore.Application.Patterns.Mediator.CheckoutMediator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/Home/Error404");
app.UseRouting();
app.UseSession(); // Use Session Middleware
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDataAsync(services);
}

app.Run();


    static async Task SeedDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OnlineStoreDbContext>();
        var userRepo = scope.ServiceProvider.GetRequiredService<DbUserRepository>();

        // Clear and re-seed every time
        context.Database.ExecuteSqlRaw("DELETE FROM ProductImages");
        context.Database.ExecuteSqlRaw("DELETE FROM Products");
        context.Database.ExecuteSqlRaw("DELETE FROM SubCategories");
        context.Database.ExecuteSqlRaw("DELETE FROM Categories");

        // Recreate users if needed
        var existingUsers = userRepo.GetAll().ToList();
        if (!existingUsers.Any())
        {
            var customer = new Customer("testuser", "test@test.com", "123 Demo St");
            customer.PasswordHash = OnlineStore.Domain.Utils.PasswordHasher.Hash("test123");
            userRepo.Add(customer);

            var admin = new Admin("admin", "admin@store.com", "SuperAdmin");
            admin.PasswordHash = OnlineStore.Domain.Utils.PasswordHasher.Hash("admin123");
            admin.Permissions.AddRange(new[] { "ManageProducts", "ManageOrders", "ManageUsers" });
            userRepo.Add(admin);
        }

        // Seed Categories with Unsplash Images
        var catClothing = new Category { Name = "Îmbrăcăminte", ImageUrl = "https://images.unsplash.com/photo-1445205170230-053b83016050?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80" };
        var catFootwear = new Category { Name = "Încălțăminte", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80" };
        var catAccessories = new Category { Name = "Accesorii", ImageUrl = "https://images.unsplash.com/photo-1511499767150-a48a237f0083?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80" };
        
        context.Categories.AddRange(catClothing, catFootwear, catAccessories);
        context.SaveChanges();

        // Optional: seed some empty subcategories to keep the structure
        var subMensClothing = new SubCategory { Name = "Haine Bărbați", CategoryId = catClothing.Id };
        var subWomensClothing = new SubCategory { Name = "Haine Femei", CategoryId = catClothing.Id };
        var subSneakers = new SubCategory { Name = "Sneakers", CategoryId = catFootwear.Id };
        var subBoots = new SubCategory { Name = "Ghete", CategoryId = catFootwear.Id };
        var subWatches = new SubCategory { Name = "Ceasuri", CategoryId = catAccessories.Id };
        var subBags = new SubCategory { Name = "Genți", CategoryId = catAccessories.Id };
        
        context.SubCategories.AddRange(subMensClothing, subWomensClothing, subSneakers, subBoots, subWatches, subBags);
        context.SaveChanges();

        // No products are added, store is completely empty!
    }

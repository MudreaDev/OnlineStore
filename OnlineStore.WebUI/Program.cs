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
    SeedData(services);
}

app.Run();

static void SeedData(IServiceProvider services)
{
    var productRepo = services.GetRequiredService<DbProductRepository>();
    var userRepo = services.GetRequiredService<DbUserRepository>();

    var existing = productRepo.GetAll().ToList();
    if (existing.Any())
    {
        foreach (var p in existing)
        {
            if (p.Name.Contains("Laptop") && string.IsNullOrEmpty(p.AvailableColors)) p.AvailableColors = "Black, Silver";
            if (p.Name.Contains("Smartphone") && string.IsNullOrEmpty(p.AvailableColors)) p.AvailableColors = "Black, White, Blue";
            if (p.Name.Contains("Headphones") && string.IsNullOrEmpty(p.AvailableColors)) p.AvailableColors = "Black, Charcoal";
            if (p.Name.Contains("T-Shirt")) 
            { 
                if (string.IsNullOrEmpty(p.AvailableColors)) p.AvailableColors = "White, Beige, Sage"; 
                if (p is ClothingProduct cp && string.IsNullOrEmpty(cp.AvailableSizes)) cp.AvailableSizes = "S, M, L, XL"; 
            }
            if (p.Name.Contains("Jeans")) 
            { 
                if (string.IsNullOrEmpty(p.AvailableColors)) p.AvailableColors = "Navy, Charcoal"; 
                if (p is ClothingProduct cp && string.IsNullOrEmpty(cp.AvailableSizes)) cp.AvailableSizes = "M, L, XL, XXL"; 
            }
            if (p.Name.Contains("Car")) 
            { 
                if (string.IsNullOrEmpty(p.AvailableColors)) p.AvailableColors = "White, Silver, Black"; 
                if (p is VehicleProduct vp && string.IsNullOrEmpty(vp.FuelType)) { vp.FuelType = "Petrol"; vp.Year = 2022; } 
            }
            if (p.Name.Contains("SUV")) 
            { 
                if (string.IsNullOrEmpty(p.AvailableColors)) p.AvailableColors = "Black, Navy"; 
                if (p is VehicleProduct vp && string.IsNullOrEmpty(vp.FuelType)) { vp.FuelType = "Diesel"; vp.Year = 2023; } 
            }
            if (p.Stock <= 0) p.Stock = 50;
            productRepo.Update(p);
        }
        return;
    }

    // Seed Products
    // Using factories just like in ConsoleUI, but manually here for simplicity or could inject factories
    // Let's manually instantiate for now as factories are simple classes
    // Seed Products
    var electronicsFactory = new ElectronicProductFactory();
    var laptop = electronicsFactory.CreateProduct("Gaming Laptop", 1500);
    laptop.AvailableColors = "Black, Silver";
    laptop.Stock = 50;
    productRepo.Add(laptop);

    var phone = electronicsFactory.CreateProduct("Smartphone", 800);
    phone.AvailableColors = "Black, White, Blue";
    phone.Stock = 50;
    productRepo.Add(phone);

    var headphones = electronicsFactory.CreateProduct("Headphones", 200);
    headphones.AvailableColors = "Black, Charcoal";
    headphones.Stock = 50;
    productRepo.Add(headphones);

    var clothingFactory = new ClothingProductFactory();
    var tshirt = (ClothingProduct)clothingFactory.CreateProduct("Cotton T-Shirt", 25);
    tshirt.AvailableSizes = "S, M, L, XL";
    tshirt.AvailableColors = "Black, White, Beige, Sage";
    tshirt.Stock = 50;
    productRepo.Add(tshirt);

    var jeans = (ClothingProduct)clothingFactory.CreateProduct("Jeans", 50);
    jeans.AvailableSizes = "M, L, XL, XXL";
    jeans.AvailableColors = "Black, Navy, Charcoal";
    jeans.Stock = 50;
    productRepo.Add(jeans);

    var vehicleFactory = new VehicleProductFactory();
    var car = (VehicleProduct)vehicleFactory.CreateProduct("City Car", 15000);
    car.FuelType = "Petrol";
    car.Year = 2022;
    car.AvailableColors = "White, Silver, Black";
    car.Stock = 50;
    productRepo.Add(car);

    var suv = (VehicleProduct)vehicleFactory.CreateProduct("SUV", 25000);
    suv.FuelType = "Diesel";
    suv.Year = 2023;
    suv.AvailableColors = "Black, Navy";
    suv.Stock = 50;
    productRepo.Add(suv);

    // Seed Customer
    var customer = new Customer("testuser", "test@test.com", "123 Demo St");
    customer.PasswordHash = OnlineStore.Domain.Utils.PasswordHasher.Hash("test123");
    userRepo.Add(customer);

    // Seed Admin
    var admin = new Admin("admin", "admin@store.com", "SuperAdmin");
    admin.PasswordHash = OnlineStore.Domain.Utils.PasswordHasher.Hash("admin123");
    admin.Permissions.AddRange(new[] { "ManageProducts", "ManageOrders", "ManageUsers" });
    userRepo.Add(admin);
}

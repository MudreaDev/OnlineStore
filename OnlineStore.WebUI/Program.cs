using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Factories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Enable Session
builder.Services.AddHttpContextAccessor();

// Register Repositories as Singletons (InMemory storage)
builder.Services.AddSingleton<InMemoryProductRepository>();
builder.Services.AddSingleton<InMemoryUserRepository>();
builder.Services.AddSingleton<InMemoryOrderRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession(); // Use Session Middleware
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Seed Data
SeedData(app.Services);

app.Run();

static void SeedData(IServiceProvider services)
{
    var productRepo = services.GetRequiredService<InMemoryProductRepository>();
    var userRepo = services.GetRequiredService<InMemoryUserRepository>();

    // Seed Products
    // Using factories just like in ConsoleUI, but manually here for simplicity or could inject factories
    // Let's manually instantiate for now as factories are simple classes
    var electronicsFactory = new ElectronicProductFactory();
    productRepo.Add(electronicsFactory.CreateProduct("Gaming Laptop", 1500));
    productRepo.Add(electronicsFactory.CreateProduct("Smartphone", 800));
    productRepo.Add(electronicsFactory.CreateProduct("Headphones", 200));

    var clothingFactory = new ClothingProductFactory();
    productRepo.Add(clothingFactory.CreateProduct("Cotton T-Shirt", 25));
    productRepo.Add(clothingFactory.CreateProduct("Jeans", 50));

    // Seed Customer
    userRepo.Add(new Customer("testuser", "test@test.com", "123 Demo St"));
}

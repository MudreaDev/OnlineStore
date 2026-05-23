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

        // Seed categories and products only if the database is empty!
        // This prevents wiping out your custom admin modifications on restart.
        if (context.Categories.Any())
        {
            return;
        }

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

        // Seed Products
        // 1. Sneakers Urban Flex
        var p1 = new FootwearProduct("Sneakers Urban Flex", 389.99m, "42", "Plasă Respirabilă & Piele Sintetică")
        {
            Stock = 25,
            AvailableColors = "Black,Red,Blue",
            AvailableSizes = "40,41,42,43,44",
            SubCategoryId = subSneakers.Id
        };
        p1.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1525966222134-fcfa99b8ae77?auto=format&fit=crop&w=800&q=80", PublicId = "sneakers_urban_flex_black", IsMain = true, DisplayOrder = 1, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=800&q=80", PublicId = "sneakers_urban_flex_red", IsMain = false, DisplayOrder = 2, AssociatedColor = "Red" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?auto=format&fit=crop&w=800&q=80", PublicId = "sneakers_urban_flex_blue", IsMain = false, DisplayOrder = 3, AssociatedColor = "Blue" }
        });

        // 2. Ghete Piele Explorer
        var p2 = new FootwearProduct("Ghete Piele Explorer", 549.99m, "43", "Piele Nubuck Premium")
        {
            Stock = 12,
            AvailableColors = "Brown,Black",
            AvailableSizes = "40,41,42,43,44,45",
            SubCategoryId = subBoots.Id
        };
        p2.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1520639888713-7851133b1ed0?auto=format&fit=crop&w=800&q=80", PublicId = "boots_explorer_brown", IsMain = true, DisplayOrder = 1, AssociatedColor = "Brown" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1608256246200-53e635b5b65f?auto=format&fit=crop&w=800&q=80", PublicId = "boots_explorer_black", IsMain = false, DisplayOrder = 2, AssociatedColor = "Black" }
        });

        // 3. Hanorac Minimalist Premium
        var p3 = new ClothingProduct("Hanorac Minimalist Premium", 249.99m, "L", "100% Bumbac Organic")
        {
            Stock = 45,
            AvailableColors = "Black,Gray,Navy",
            AvailableSizes = "S,M,L,XL,XXL",
            SubCategoryId = subMensClothing.Id
        };
        p3.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1556821840-3a63f95609a7?auto=format&fit=crop&w=800&q=80", PublicId = "hoodie_minimalist_black", IsMain = true, DisplayOrder = 1, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1566206091558-7f218b696731?auto=format&fit=crop&w=800&q=80", PublicId = "hoodie_minimalist_gray", IsMain = false, DisplayOrder = 2, AssociatedColor = "Gray" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1620799140408-edc6dcb6d633?auto=format&fit=crop&w=800&q=80", PublicId = "hoodie_minimalist_navy", IsMain = false, DisplayOrder = 3, AssociatedColor = "Navy" }
        });

        // 4. Rochie Elegantă de Seară
        var p4 = new ClothingProduct("Rochie Elegantă de Seară", 489.99m, "S", "Mătase Naturală & Satin")
        {
            Stock = 8,
            AvailableColors = "Red,Black,Green",
            AvailableSizes = "XS,S,M,L",
            SubCategoryId = subWomensClothing.Id
        };
        p4.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1595777457583-95e059d581b8?auto=format&fit=crop&w=800&q=80", PublicId = "dress_elegant_red", IsMain = true, DisplayOrder = 1, AssociatedColor = "Red" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1496747611176-843222e1e57c?auto=format&fit=crop&w=800&q=80", PublicId = "dress_elegant_black", IsMain = false, DisplayOrder = 2, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1539008885759-c3b85194852c?auto=format&fit=crop&w=800&q=80", PublicId = "dress_elegant_green", IsMain = false, DisplayOrder = 3, AssociatedColor = "Green" }
        });

        // 5. Ceas Chronograph Classic
        var p5 = new AccessoryProduct("Ceas Chronograph Classic", 999.99m, "Chrono-X Luxury", "Oțel Inoxidabil & Sticlă Safir")
        {
            Stock = 15,
            AvailableColors = "Gold,Silver,Black",
            SubCategoryId = subWatches.Id
        };
        p5.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1522312346375-d1a52e2b99b3?auto=format&fit=crop&w=800&q=80", PublicId = "watch_chrono_gold", IsMain = true, DisplayOrder = 1, AssociatedColor = "Gold" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1547996160-81dfa63595aa?auto=format&fit=crop&w=800&q=80", PublicId = "watch_chrono_silver", IsMain = false, DisplayOrder = 2, AssociatedColor = "Silver" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1524805444758-089113d48a6d?auto=format&fit=crop&w=800&q=80", PublicId = "watch_chrono_black", IsMain = false, DisplayOrder = 3, AssociatedColor = "Black" }
        });

        // 6. Geantă Piele Couture
        var p6 = new AccessoryProduct("Geantă Piele Couture", 649.99m, "Couture Paris", "Piele Saffiano")
        {
            Stock = 18,
            AvailableColors = "Tan,Black,Red",
            SubCategoryId = subBags.Id
        };
        p6.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1584917865442-de89df76afd3?auto=format&fit=crop&w=800&q=80", PublicId = "bag_couture_tan", IsMain = true, DisplayOrder = 1, AssociatedColor = "Tan" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1590874103328-eac38a683ce7?auto=format&fit=crop&w=800&q=80", PublicId = "bag_couture_black", IsMain = false, DisplayOrder = 2, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1566150905458-1bf1fc15a490?auto=format&fit=crop&w=800&q=80", PublicId = "bag_couture_red", IsMain = false, DisplayOrder = 3, AssociatedColor = "Red" }
        });

        // 7. Tricou Premium Minimalist
        var p7 = new ClothingProduct("Tricou Premium Minimalist", 129.99m, "M", "100% Bumbac Supima")
        {
            Stock = 50,
            AvailableColors = "White,Black,Olive",
            AvailableSizes = "S,M,L,XL",
            SubCategoryId = subMensClothing.Id
        };
        p7.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1521572267360-ee0c2909d518?auto=format&fit=crop&w=800&q=80", PublicId = "tshirt_minimalist_white", IsMain = true, DisplayOrder = 1, AssociatedColor = "White" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1503342217505-b0a15ec3261c?auto=format&fit=crop&w=800&q=80", PublicId = "tshirt_minimalist_black", IsMain = false, DisplayOrder = 2, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1581655353564-df123a1eb820?auto=format&fit=crop&w=800&q=80", PublicId = "tshirt_minimalist_olive", IsMain = false, DisplayOrder = 3, AssociatedColor = "Olive" }
        });

        // 8. Palton de Lână Elegance
        var p8 = new ClothingProduct("Palton de Lână Elegance", 799.99m, "M", "80% Lână Virgină & 20% Cașmir")
        {
            Stock = 10,
            AvailableColors = "Tan,Black,Gray",
            AvailableSizes = "XS,S,M,L,XL",
            SubCategoryId = subWomensClothing.Id
        };
        p8.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?auto=format&fit=crop&w=800&q=80", PublicId = "coat_elegance_tan", IsMain = true, DisplayOrder = 1, AssociatedColor = "Tan" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?auto=format&fit=crop&w=800&q=80", PublicId = "coat_elegance_black", IsMain = false, DisplayOrder = 2, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?auto=format&fit=crop&w=800&q=80", PublicId = "coat_elegance_gray", IsMain = false, DisplayOrder = 3, AssociatedColor = "Gray" }
        });

        // 9. Sneakers Retro Classic
        var p9 = new FootwearProduct("Sneakers Retro Classic", 329.99m, "41", "Piele Ecologică & Talpă Gumă")
        {
            Stock = 30,
            AvailableColors = "White,Green,Red",
            AvailableSizes = "39,40,41,42,43,44",
            SubCategoryId = subSneakers.Id
        };
        p9.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1600185365483-26d7a4cc7519?auto=format&fit=crop&w=800&q=80", PublicId = "sneakers_retro_white", IsMain = true, DisplayOrder = 1, AssociatedColor = "White" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1597045566677-8cf032ed6634?auto=format&fit=crop&w=800&q=80", PublicId = "sneakers_retro_green", IsMain = false, DisplayOrder = 2, AssociatedColor = "Green" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1606107557195-0e29a4b5b4aa?auto=format&fit=crop&w=800&q=80", PublicId = "sneakers_retro_red", IsMain = false, DisplayOrder = 3, AssociatedColor = "Red" }
        });

        // 10. Ghete Chelsea Premium
        var p10 = new FootwearProduct("Ghete Chelsea Premium", 499.99m, "42", "Piele Întoarsă (Suede)")
        {
            Stock = 15,
            AvailableColors = "Tan,Black",
            AvailableSizes = "39,40,41,42,43,44",
            SubCategoryId = subBoots.Id
        };
        p10.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1603487988353-c8e4b3017a9a?auto=format&fit=crop&w=800&q=80", PublicId = "boots_chelsea_tan", IsMain = true, DisplayOrder = 1, AssociatedColor = "Tan" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1638247025967-b4e38f787b76?auto=format&fit=crop&w=800&q=80", PublicId = "boots_chelsea_black", IsMain = false, DisplayOrder = 2, AssociatedColor = "Black" }
        });

        // 11. Ceas Minimalist Slim
        var p11 = new AccessoryProduct("Ceas Minimalist Slim", 699.99m, "Nordic Design", "Carcasă Ultra-Subțire din Titan & Curea Piele")
        {
            Stock = 20,
            AvailableColors = "Black,Silver",
            SubCategoryId = subWatches.Id
        };
        p11.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?auto=format&fit=crop&w=800&q=80", PublicId = "watch_minimalist_black", IsMain = true, DisplayOrder = 1, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1612817288484-6f916006741a?auto=format&fit=crop&w=800&q=80", PublicId = "watch_minimalist_silver", IsMain = false, DisplayOrder = 2, AssociatedColor = "Silver" }
        });

        // 12. Rucsac Urban Scout
        var p12 = new AccessoryProduct("Rucsac Urban Scout", 349.99m, "Scout Utility", "Cordura Impermeabil & Detalii Piele")
        {
            Stock = 25,
            AvailableColors = "Olive,Navy,Black",
            SubCategoryId = subBags.Id
        };
        p12.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?auto=format&fit=crop&w=800&q=80", PublicId = "backpack_scout_olive", IsMain = true, DisplayOrder = 1, AssociatedColor = "Olive" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1581605405669-fcdf81165afa?auto=format&fit=crop&w=800&q=80", PublicId = "backpack_scout_navy", IsMain = false, DisplayOrder = 2, AssociatedColor = "Navy" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1577733966973-d680bffd2e80?auto=format&fit=crop&w=800&q=80", PublicId = "backpack_scout_black", IsMain = false, DisplayOrder = 3, AssociatedColor = "Black" }
        });

        // 13. Cămașă de In Premium
        var p13 = new ClothingProduct("Cămașă de In Premium", 179.99m, "L", "100% In Premium")
        {
            Stock = 25,
            AvailableColors = "White,Navy,Olive",
            AvailableSizes = "S,M,L,XL,XXL",
            SubCategoryId = subMensClothing.Id
        };
        p13.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?auto=format&fit=crop&w=800&q=80", PublicId = "shirt_linen_white", IsMain = true, DisplayOrder = 1, AssociatedColor = "White" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1602810318383-e386cc2a3ccf?auto=format&fit=crop&w=800&q=80", PublicId = "shirt_linen_navy", IsMain = false, DisplayOrder = 2, AssociatedColor = "Navy" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1607345366928-199ea26cfe3e?auto=format&fit=crop&w=800&q=80", PublicId = "shirt_linen_olive", IsMain = false, DisplayOrder = 3, AssociatedColor = "Olive" }
        });

        // 14. Pantaloni Chino Slim
        var p14 = new ClothingProduct("Pantaloni Chino Slim", 199.99m, "32", "98% Bumbac & 2% Elastan")
        {
            Stock = 35,
            AvailableColors = "Khaki,Navy,Black",
            AvailableSizes = "30,32,34,36",
            SubCategoryId = subMensClothing.Id
        };
        p14.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1624378439575-d8705ad7ae80?auto=format&fit=crop&w=800&q=80", PublicId = "pants_chino_khaki", IsMain = true, DisplayOrder = 1, AssociatedColor = "Khaki" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?auto=format&fit=crop&w=800&q=80", PublicId = "pants_chino_navy", IsMain = false, DisplayOrder = 2, AssociatedColor = "Navy" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1506629082925-6fc6c7b25c15?auto=format&fit=crop&w=800&q=80", PublicId = "pants_chino_black", IsMain = false, DisplayOrder = 3, AssociatedColor = "Black" }
        });

        // 15. Cardigan Călduros de Iarnă
        var p15 = new ClothingProduct("Cardigan Călduros de Iarnă", 289.99m, "M", "50% Lână, 30% Acrilic & 20% Poliamidă")
        {
            Stock = 15,
            AvailableColors = "Gray,Beige,Black",
            AvailableSizes = "S,M,L,XL",
            SubCategoryId = subWomensClothing.Id
        };
        p15.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1583743814966-8936f5b7be1a?auto=format&fit=crop&w=800&q=80", PublicId = "cardigan_cozy_gray", IsMain = true, DisplayOrder = 1, AssociatedColor = "Gray" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1614975058789-41316d0e2e9c?auto=format&fit=crop&w=800&q=80", PublicId = "cardigan_cozy_beige", IsMain = false, DisplayOrder = 2, AssociatedColor = "Beige" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?auto=format&fit=crop&w=800&q=80", PublicId = "cardigan_cozy_black", IsMain = false, DisplayOrder = 3, AssociatedColor = "Black" }
        });

        // 16. Sacou Smart Casual
        var p16 = new ClothingProduct("Sacou Smart Casual", 349.99m, "S", "Poliester Premium & Viscoză")
        {
            Stock = 18,
            AvailableColors = "Pink,Black,White",
            AvailableSizes = "XS,S,M,L",
            SubCategoryId = subWomensClothing.Id
        };
        p16.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1585487000160-6ebcfceb0d03?auto=format&fit=crop&w=800&q=80", PublicId = "blazer_smart_pink", IsMain = true, DisplayOrder = 1, AssociatedColor = "Pink" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1548624149-f7b2e6c7038e?auto=format&fit=crop&w=800&q=80", PublicId = "blazer_smart_black", IsMain = false, DisplayOrder = 2, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1594633312681-425c7b97ccd1?auto=format&fit=crop&w=800&q=80", PublicId = "blazer_smart_white", IsMain = false, DisplayOrder = 3, AssociatedColor = "White" }
        });

        // 17. Sneakers Air Speed
        var p17 = new FootwearProduct("Sneakers Air Speed", 459.99m, "43", "Fibre Sintetice Aero & Pernă de Aer")
        {
            Stock = 22,
            AvailableColors = "White,Black,Orange",
            AvailableSizes = "40,41,42,43,44,45",
            SubCategoryId = subSneakers.Id
        };
        p17.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1539185441755-769473a23570?auto=format&fit=crop&w=800&q=80", PublicId = "sneakers_air_white", IsMain = true, DisplayOrder = 1, AssociatedColor = "White" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1508609348619-7480a70018e6?auto=format&fit=crop&w=800&q=80", PublicId = "sneakers_air_black", IsMain = false, DisplayOrder = 2, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1515955656352-a1fa3ffcd111?auto=format&fit=crop&w=800&q=80", PublicId = "sneakers_air_orange", IsMain = false, DisplayOrder = 3, AssociatedColor = "Orange" }
        });

        // 18. Mocasini din Piele Întoarsă
        var p18 = new FootwearProduct("Mocasini din Piele Întoarsă", 379.99m, "42", "100% Piele Întoarsă Naturală")
        {
            Stock = 16,
            AvailableColors = "Navy,Brown",
            AvailableSizes = "40,41,42,43,44",
            SubCategoryId = subBoots.Id
        };
        p18.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1533867617858-e7b97e060509?auto=format&fit=crop&w=800&q=80", PublicId = "moccasins_suede_navy", IsMain = true, DisplayOrder = 1, AssociatedColor = "Navy" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1549298916-b41d501d3772?auto=format&fit=crop&w=800&q=80", PublicId = "moccasins_suede_brown", IsMain = false, DisplayOrder = 2, AssociatedColor = "Brown" }
        });

        // 19. Ceas Chrono Sport
        var p19 = new AccessoryProduct("Ceas Chrono Sport", 799.99m, "Active Time", "Carcasă Policarbonat Dur & Curea Silicon")
        {
            Stock = 25,
            AvailableColors = "Black,Red",
            SubCategoryId = subWatches.Id
        };
        p19.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1517466807914-9989af326f0f?auto=format&fit=crop&w=800&q=80", PublicId = "watch_sport_black", IsMain = true, DisplayOrder = 1, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1557531382-c8b613be539d?auto=format&fit=crop&w=800&q=80", PublicId = "watch_sport_red", IsMain = false, DisplayOrder = 2, AssociatedColor = "Red" }
        });

        // 20. Ceas Vintage Gold
        var p20 = new AccessoryProduct("Ceas Vintage Gold", 1199.99m, "Heritage Swiss", "Placat Aur 18K & Curea Piele Aligator")
        {
            Stock = 8,
            AvailableColors = "Gold,Brown",
            SubCategoryId = subWatches.Id
        };
        p20.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1526049618242-90a42674a41d?auto=format&fit=crop&w=800&q=80", PublicId = "watch_vintage_gold", IsMain = true, DisplayOrder = 1, AssociatedColor = "Gold" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1539874754764-5a96559165b0?auto=format&fit=crop&w=800&q=80", PublicId = "watch_vintage_brown", IsMain = false, DisplayOrder = 2, AssociatedColor = "Brown" }
        });

        // 21. Servietă din Piele Business
        var p21 = new AccessoryProduct("Servietă din Piele Business", 589.99m, "Milano Leather", "Piele Full Grain")
        {
            Stock = 12,
            AvailableColors = "Brown,Black",
            SubCategoryId = subBags.Id
        };
        p21.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1548036328-c9fa89d128fa?auto=format&fit=crop&w=800&q=80", PublicId = "briefcase_leather_brown", IsMain = true, DisplayOrder = 1, AssociatedColor = "Brown" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1594223274512-ad4803739b7c?auto=format&fit=crop&w=800&q=80", PublicId = "briefcase_leather_black", IsMain = false, DisplayOrder = 2, AssociatedColor = "Black" }
        });

        // 22. Portofel Slim din Piele
        var p22 = new AccessoryProduct("Portofel Slim din Piele", 149.99m, "Minimalist Co.", "Piele Naturală Saffiano cu protecție RFID")
        {
            Stock = 40,
            AvailableColors = "Black,Brown",
            SubCategoryId = subBags.Id
        };
        p22.Images.AddRange(new[]
        {
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1627124718515-552fd41149f2?auto=format&fit=crop&w=800&q=80", PublicId = "wallet_leather_black", IsMain = true, DisplayOrder = 1, AssociatedColor = "Black" },
            new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1606503837279-e36d802950ec?auto=format&fit=crop&w=800&q=80", PublicId = "wallet_leather_brown", IsMain = false, DisplayOrder = 2, AssociatedColor = "Brown" }
        });

        context.Products.AddRange(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22);
        context.SaveChanges();
    }

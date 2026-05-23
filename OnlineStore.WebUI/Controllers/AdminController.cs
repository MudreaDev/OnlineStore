using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Application.Repositories;
using OnlineStore.Application.Data;
using OnlineStore.Application.Services;
using OnlineStore.Application.Patterns.Visitor;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Factories;
using OnlineStore.Domain.Singleton;
using OnlineStore.Domain.DesignPatterns.Structural.Proxy;
using OnlineStore.Domain.DesignPatterns.Structural.Decorator;
using OnlineStore.Domain.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace OnlineStore.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly DbProductRepository _productRepo;
        private readonly DbOrderRepository _orderRepo;
        private readonly DbUserRepository _userRepo;
        private readonly CloudinaryService _cloudinaryService;
        private readonly OnlineStoreDbContext _context;
        private readonly OrderNotificationService _notificationService;
        private readonly ProductAvailabilityService _availabilityService;
        private readonly ToastService _toastService;

        public AdminController(
            DbProductRepository productRepo, 
            DbOrderRepository orderRepo, 
            DbUserRepository userRepo,
            CloudinaryService cloudinaryService,
            OnlineStoreDbContext context,
            OrderNotificationService notificationService,
            ProductAvailabilityService availabilityService,
            ToastService toastService)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _cloudinaryService = cloudinaryService;
            _context = context;
            _notificationService = notificationService;
            _availabilityService = availabilityService;
            _toastService = toastService;
        }

        private IActionResult? CheckAccess()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            var user = Guid.TryParse(userIdStr, out Guid userId)
                ? _userRepo.GetById(userId) : null;

            var role = user is Admin ? "Admin" : "Customer";

            // Proxy Pattern — ProtectionProxy controlează accesul în loc de IsAdmin() manual
            IResource proxy = new ProtectionProxy();
            var accessResult = proxy.Access(role);

            if (accessResult.Contains("interzis"))
            {
                TempData["Error"] = accessResult;
                return RedirectToAction("Login", "Account");
            }

            return null;
        }

        public IActionResult Index()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var products = _productRepo.GetAll().ToList();
            var orders = _orderRepo.GetAll().ToList();
            
            ViewBag.TotalProducts = products.Count;
            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalRevenue = orders.Sum(o => o.Total);
            ViewBag.LowStock = products.Count(p => p.Stock < 5);

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.SubCategories = _context.SubCategories.Select(s => new { 
                Id = s.Id, 
                Name = s.Name, 
                ParentName = s.Category.Name,
                Attributes = s.ExpectedAttributes 
            }).ToList();

            return View();
        }

        public IActionResult Products(int page = 1)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            const int pageSize = 20;
            var allProducts = _productRepo.GetAll().ToList();

            var paginatedProducts = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(allProducts.Count / (double)pageSize);

            return View(paginatedProducts);
        }

        public IActionResult Orders()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var orders = _orderRepo.GetAll();
            return View(orders);
        }        public IActionResult SalesStatistics()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var orders = _orderRepo.GetAll().ToList();
            
            // Calculate key metrics
            var activeOrders = orders.Where(o => o.Status != OrderStatus.Cancelled).ToList();
            
            ViewBag.TotalSales = activeOrders.Sum(o => o.Total);
            ViewBag.TotalOrdersCount = orders.Count;
            ViewBag.ActiveOrdersCount = activeOrders.Count;
            ViewBag.AverageOrderValue = activeOrders.Any() ? activeOrders.Average(o => o.Total) : 0;
            ViewBag.TotalUnitsSold = activeOrders.SelectMany(o => o.Items).Sum(i => i.Quantity);
            
            // Sales by Status
            var salesByStatus = orders.GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count(), Revenue = g.Sum(o => o.Total) })
                .ToList();
            
            ViewBag.SalesByStatusPendingCount = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Pending)?.Count ?? 0;
            ViewBag.SalesByStatusPendingRevenue = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Pending)?.Revenue ?? 0;
            
            ViewBag.SalesByStatusPaidCount = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Paid)?.Count ?? 0;
            ViewBag.SalesByStatusPaidRevenue = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Paid)?.Revenue ?? 0;
            
            ViewBag.SalesByStatusShippedCount = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Shipped)?.Count ?? 0;
            ViewBag.SalesByStatusShippedRevenue = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Shipped)?.Revenue ?? 0;
            
            ViewBag.SalesByStatusDeliveredCount = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Delivered)?.Count ?? 0;
            ViewBag.SalesByStatusDeliveredRevenue = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Delivered)?.Revenue ?? 0;
            
            ViewBag.SalesByStatusCanceledCount = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Cancelled)?.Count ?? 0;
            ViewBag.SalesByStatusCanceledRevenue = salesByStatus.FirstOrDefault(s => s.Status == OrderStatus.Cancelled)?.Revenue ?? 0;

            // Top Selling Products
            var topProducts = activeOrders.SelectMany(o => o.Items)
                .GroupBy(i => new { i.ProductId, i.ProductName })
                .Select(g => new { 
                    ProductId = g.Key.ProductId, 
                    ProductName = g.Key.ProductName, 
                    UnitsSold = g.Sum(i => i.Quantity), 
                    Revenue = g.Sum(i => i.Quantity * i.UnitPrice) 
                })
                .OrderByDescending(p => p.UnitsSold)
                .Take(5)
                .ToList();
            
            var topProductsList = new List<Tuple<string, int, decimal>>();
            foreach (var p in topProducts)
            {
                topProductsList.Add(Tuple.Create(p.ProductName, p.UnitsSold, p.Revenue));
            }
            ViewBag.TopProducts = topProductsList;

            // Sales by Product Category
            var productsDict = _context.Products
                .Include(p => p.SubCategory)
                .ThenInclude(sc => sc!.Category)
                .Select(p => new { p.Id, CategoryName = p.SubCategory != null && p.SubCategory.Category != null ? p.SubCategory.Category.Name : "Altele" })
                .ToDictionary(p => p.Id, p => p.CategoryName);

            var categorySales = activeOrders.SelectMany(o => o.Items)
                .Select(i => new { 
                    CategoryName = productsDict.TryGetValue(i.ProductId, out var cat) ? cat : "Altele", 
                    Quantity = i.Quantity, 
                    Revenue = i.Quantity * i.UnitPrice 
                })
                .GroupBy(i => i.CategoryName)
                .Select(g => new { 
                    CategoryName = g.Key, 
                    UnitsSold = g.Sum(i => i.Quantity), 
                    Revenue = g.Sum(i => i.Revenue) 
                })
                .OrderByDescending(c => c.Revenue)
                .ToList();

            var categorySalesList = new List<Tuple<string, int, decimal>>();
            foreach (var c in categorySales)
            {
                categorySalesList.Add(Tuple.Create(c.CategoryName, c.UnitsSold, c.Revenue));
            }
            ViewBag.CategorySales = categorySalesList;

            // Sales trends over time (Group by date)
            var dailySales = activeOrders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.Total), Count = g.Count() })
                .OrderByDescending(d => d.Date)
                .Take(10)
                .OrderBy(d => d.Date)
                .ToList();

            var dailySalesList = new List<Tuple<string, int, decimal>>();
            foreach (var d in dailySales)
            {
                dailySalesList.Add(Tuple.Create(d.Date.ToString("dd MMM"), d.Count, d.Revenue));
            }
            ViewBag.DailySales = dailySalesList;

            return View();
        }
        [HttpGet]
        public IActionResult AddProduct()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(string type, string name, decimal price, int? stock, string size, string material, string availableSizes, string availableColors, string brand, List<IFormFile>? images, int mainImageIndex = 0, List<string>? newImageColors = null)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            ProductFactory? factory = type switch
            {
                "Footwear" => new FootwearProductFactory(),
                "Clothing" => new ClothingProductFactory(),
                "Accessory" => new AccessoryProductFactory(),
                "Dynamic" => new DynamicProductFactory(),
                _ => null
            };

            if (factory != null)
            {
                var newProduct = factory.CreateProduct(name, price);
                newProduct.Stock = stock ?? 10;

                newProduct.AvailableColors = availableColors;

                if (newProduct is FootwearProduct footwear)
                {
                    if (!string.IsNullOrEmpty(size)) footwear.Size = size;
                    if (!string.IsNullOrEmpty(material)) footwear.Material = material;
                    footwear.AvailableSizes = availableSizes;
                }
                else if (newProduct is ClothingProduct clothing)
                {
                    if (!string.IsNullOrEmpty(size)) clothing.Size = size;
                    if (!string.IsNullOrEmpty(material)) clothing.Material = material;
                    clothing.AvailableSizes = availableSizes;
                }
                else if (newProduct is AccessoryProduct accessory)
                {
                    if (!string.IsNullOrEmpty(brand)) accessory.Brand = brand;
                    if (!string.IsNullOrEmpty(material)) accessory.Material = material;
                }
                else if (newProduct is DynamicProduct dynamicProduct)
                {
                    if (Guid.TryParse(HttpContext.Request.Form["subcategoryId"], out Guid subcatId))
                        dynamicProduct.SubCategoryId = subcatId;

                    foreach (var key in HttpContext.Request.Form.Keys.Where(k => k.StartsWith("dyn_")))
                    {
                        var attrKey = key.Substring(4);
                        var value = HttpContext.Request.Form[key].ToString();
                        if (value != null) dynamicProduct.CustomAttributes[attrKey] = value;
                    }
                }

                _productRepo.Add(newProduct);

                if (images != null && images.Any())
                {
                    try
                    {
                        var uploadCount = Math.Min(images.Count, 5);
                        for (int i = 0; i < uploadCount; i++)
                        {
                            var file = images[i];
                            var uploadResult = await _cloudinaryService.UploadImageAsync(file);
                            
                            var productImage = new ProductImage
                            {
                                ProductId = newProduct.Id,
                                ImageUrl = uploadResult.Url,
                                PublicId = uploadResult.PublicId,
                                IsMain = (i == mainImageIndex),
                                DisplayOrder = i,
                                AssociatedColor = newImageColors != null && newImageColors.Count > i ? newImageColors[i] : null
                            };
                            
                            _context.ProductImages.Add(productImage);
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Eroare la încărcarea imaginilor: {ex.Message}";
                    }
                }
                TempData["Success"] = "Produsul a fost adăugat cu succes!";
            }

            return RedirectToAction("Products");
        }

        [HttpGet]
        public IActionResult GetSubcategories()
        {
            var subs = _context.SubCategories.Select(s => new {
                id = s.Id,
                name = s.Name,
                expectedAttributes = s.ExpectedAttributes
            }).ToList();
            return Json(subs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(string categoryName, IFormFile? categoryImage, bool isFeatured = true)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = new Category 
                { 
                    Name = categoryName,
                    IsFeatured = isFeatured
                };

                if (categoryImage != null)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(categoryImage);
                    category.ImageUrl = uploadResult.Url;
                    category.PublicId = uploadResult.PublicId;
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Imediat după ce se creează și se salvează cu succes noua entitate Category,
                // creăm automat o subcategorie implicită asociată acesteia.
                var sub = new SubCategory
                {
                    CategoryId = category.Id,
                    Name = category.Name,
                    ExpectedAttributes = ""
                };
                _context.SubCategories.Add(sub);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Categoria vizuală '{categoryName}' a fost creată!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Guid categoryId, string categoryName, IFormFile? categoryImage)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                if (!string.IsNullOrEmpty(categoryName))
                {
                    var oldName = category.Name;
                    category.Name = categoryName;

                    // Dacă există o subcategorie cu numele vechi al categoriei din această categorie, o redenumim
                    var defaultSub = _context.SubCategories.FirstOrDefault(s => s.CategoryId == category.Id && s.Name == oldName);
                    if (defaultSub != null)
                    {
                        defaultSub.Name = categoryName;
                    }
                }

                if (categoryImage != null)
                {
                    // delete old Cloudinary image if it exists
                    if (!string.IsNullOrEmpty(category.PublicId))
                    {
                        try
                        {
                            await _cloudinaryService.DeleteImageAsync(category.PublicId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting old category image: {ex.Message}");
                        }
                    }

                    var uploadResult = await _cloudinaryService.UploadImageAsync(categoryImage);
                    category.ImageUrl = uploadResult.Url;
                    category.PublicId = uploadResult.PublicId;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Categoria '{category.Name}' a fost actualizată cu succes!";
            }
            else
            {
                TempData["Error"] = "Categoria nu a fost găsită.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateTaxonomy(Guid parentCategoryId, string taxonomyName, string attributesCsv)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            if (!string.IsNullOrEmpty(taxonomyName))
            {
                var sub = new SubCategory
                {
                    Name = taxonomyName,
                    ExpectedAttributes = attributesCsv,
                    CategoryId = parentCategoryId
                };

                _context.SubCategories.Add(sub);
                _context.SaveChanges();
                TempData["Success"] = $"Sub-categoria '{taxonomyName}' a fost creată cu succes!";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditProduct(Guid id)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Guid id, string name, decimal price, int? stock, string size, string material, string availableSizes, string availableColors, string brand, List<IFormFile>? newImages, List<string>? deletePublicIds, string? mainImagePublicId, int? mainNewImageIndex, List<string>? imagePublicIds, List<string>? imageColors, List<string>? newImageColors)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            var oldStock = product.Stock;
            product.Name = name;
            product.Price = price;
            product.Stock = stock ?? product.Stock;
            product.AvailableColors = availableColors;

            if (product is FootwearProduct footwearUpdate)
            {
                footwearUpdate.Size = size;
                footwearUpdate.Material = material;
                footwearUpdate.AvailableSizes = availableSizes;
            }
            else if (product is ClothingProduct clothingUpdate)
            {
                clothingUpdate.Size = size;
                clothingUpdate.Material = material;
                clothingUpdate.AvailableSizes = availableSizes;
            }
            else if (product is AccessoryProduct accessoryUpdate)
            {
                accessoryUpdate.Brand = brand;
                accessoryUpdate.Material = material;
            }

            // Notification: Back in stock logic
            if (oldStock == 0 && product.Stock > 0)
            {
                _availabilityService.NotifyBackInStock(product);
            }

            // Handle deletions
            if (deletePublicIds != null && deletePublicIds.Any())
            {
                foreach (var publicId in deletePublicIds)
                {
                    var img = _context.ProductImages.FirstOrDefault(pi => pi.PublicId == publicId);
                    if (img != null)
                    {
                        await _cloudinaryService.DeleteImageAsync(publicId);
                        _context.ProductImages.Remove(img);
                    }
                }
                await _context.SaveChangesAsync();
            }

            // Handle Master Image selection for existing images
            if (!string.IsNullOrEmpty(mainImagePublicId))
            {
                var allImgs = _context.ProductImages.Where(pi => pi.ProductId == id).ToList();
                foreach (var img in allImgs)
                {
                    img.IsMain = (img.PublicId == mainImagePublicId);
                }
                await _context.SaveChangesAsync();
            }

            // Update existing image colors
            if (imagePublicIds != null && imageColors != null)
            {
                for (int i = 0; i < Math.Min(imagePublicIds.Count, imageColors.Count); i++)
                {
                    var pubId = imagePublicIds[i];
                    var col = imageColors[i];
                    var img = _context.ProductImages.FirstOrDefault(pi => pi.ProductId == id && pi.PublicId == pubId);
                    if (img != null)
                    {
                        img.AssociatedColor = col;
                    }
                }
                await _context.SaveChangesAsync();
            }

            // Handle new uploads
            if (newImages != null && newImages.Any())
            {
                var existingCount = _context.ProductImages.Count(pi => pi.ProductId == id);
                var availableSlots = Math.Max(0, 5 - existingCount);
                var uploadCount = Math.Min(newImages.Count, availableSlots);

                if (uploadCount > 0)
                {
                    try
                    {
                        for (int i = 0; i < uploadCount; i++)
                        {
                            var file = newImages[i];
                            var uploadResult = await _cloudinaryService.UploadImageAsync(file);
                            
                            var productImage = new ProductImage
                            {
                                ProductId = id,
                                ImageUrl = uploadResult.Url,
                                PublicId = uploadResult.PublicId,
                                IsMain = (i == mainNewImageIndex && string.IsNullOrEmpty(mainImagePublicId)),
                                DisplayOrder = existingCount + i,
                                AssociatedColor = newImageColors != null && newImageColors.Count > i ? newImageColors[i] : null
                            };
                            
                            _context.ProductImages.Add(productImage);
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Eroare la încărcarea noilor imagini: {ex.Message}";
                    }
                }
            }

            _productRepo.Update(product);
            TempData["Success"] = "Produsul a fost actualizat!";
            return RedirectToAction("Products");
        }

        [HttpPost]
        public IActionResult DeleteProduct(Guid id)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            _productRepo.Delete(id);
            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(Guid imageId)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var image = _context.ProductImages.Find(imageId);
            if (image != null)
            {
                await _cloudinaryService.DeleteImageAsync(image.PublicId);
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Imaginea a fost ștearsă.";
            }
            else
            {
                TempData["Error"] = "Imaginea nu a fost găsită.";
            }

            return RedirectToAction("Products");
        }

        [HttpPost]
        public IActionResult CloneProduct(Guid id)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            Product clone = product switch
            {
                FootwearProduct fp => fp.Clone(),
                ClothingProduct cp => cp.Clone(),
                AccessoryProduct ap => ap.Clone(),
                _ => throw new NotSupportedException("Product type not supported for cloning.")
            };

            _productRepo.Add(clone);
            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCategoryFeatured(Guid id)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.IsFeatured = !category.IsFeatured;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Statusul categoriei '{category.Name}' a fost actualizat.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(Guid orderId, OnlineStore.Domain.Enums.OrderStatus status)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var order = _orderRepo.GetById(orderId);
            if (order != null)
            {
                // Observer Pattern — subiectul actualizează starea și notifică automat observatorii
                _notificationService.UpdateStatusAndNotify(order, status);
                _orderRepo.Update(order);

                // Decorator Pattern — construiește lanțul de notificări compuse
                INotification notifier = new EmailNotification();
                if (status == OrderStatus.Shipped || status == OrderStatus.Delivered)
                    notifier = new SmsDecorator(notifier);
                if (status == OrderStatus.Delivered)
                    notifier = new PushDecorator(notifier);

                var decoratorResult = notifier.Send(
                    $"Comanda {orderId.ToString()[..8]} este acum {status}");
                TempData["NotificationChain"] = decoratorResult;

                _toastService.AddToast($"Statusul comenzii {orderId.ToString()[..8]} a fost actualizat la {status}!");
            }
            else
            {
                TempData["Error"] = "Comanda nu a fost găsită.";
            }

            return RedirectToAction("Orders");
        }

        [HttpPost]
        public IActionResult UpdateSettings(string storeName, decimal vatPercentage, decimal freeShippingThreshold)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            ApplicationConfigurationManager.Instance.UpdateSettings(storeName, vatPercentage, freeShippingThreshold);

            TempData["Message"] = "Setările magazinului au fost actualizate.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ExportInventory()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var products = _productRepo.GetAll();
            var visitor = new ProductExportVisitor();

            foreach (var product in products)
            {
                product.Accept(visitor);
            }

            var exportData = visitor.GetExportResult();
            var fileName = $"InventoryExport_{DateTime.Now:yyyyMMdd}.txt";
            
            return File(Encoding.UTF8.GetBytes(exportData), "text/plain", fileName);
        }

        [HttpPost]
        public IActionResult ShipOrderState(Guid id)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var order = _orderRepo.GetById(id);
            if (order != null)
            {
                try
                {
                    order.Ship(); // Calls the State transition
                    _orderRepo.Update(order);
                    
                    // Trigger Notification (Observer Pattern)
                    _notificationService.UpdateStatusAndNotify(order, order.Status);
                    
                    TempData["Success"] = $"Comanda {id.ToString()[..8]} a fost expediată (State Pattern).";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Eroare (Stat): {ex.Message}";
                }
            }
            return RedirectToAction("Orders");
        }

        [HttpPost]
        public IActionResult CancelOrderState(Guid id)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var order = _orderRepo.GetById(id);
            if (order != null)
            {
                try
                {
                    order.Cancel(); // Calls the State transition
                    _orderRepo.Update(order);
                    
                    // Trigger Notification (Observer Pattern)
                    _notificationService.UpdateStatusAndNotify(order, order.Status);
                    
                    TempData["Success"] = $"Comanda {id.ToString()[..8]} a fost anulată (State Pattern).";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Eroare (Stat): {ex.Message}";
                }
            }
            return RedirectToAction("Orders");
        }
    }
}

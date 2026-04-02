using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OnlineStore.Application.Repositories;
using OnlineStore.Application.Data;
using OnlineStore.Application.Services;
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

        public AdminController(
            DbProductRepository productRepo, 
            DbOrderRepository orderRepo, 
            DbUserRepository userRepo,
            CloudinaryService cloudinaryService,
            OnlineStoreDbContext context,
            OrderNotificationService notificationService,
            ProductAvailabilityService availabilityService)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _cloudinaryService = cloudinaryService;
            _context = context;
            _notificationService = notificationService;
            _availabilityService = availabilityService;
        }

        private IActionResult CheckAccess()
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
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(string type, string name, decimal price, int? stock, int? warrantyMonths, string size, string material, string availableSizes, string availableColors, string brand, string model, int? year, List<IFormFile>? images)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            ProductFactory factory = type switch
            {
                "Electronic" => new ElectronicProductFactory(),
                "Clothing" => new ClothingProductFactory(),
                "Vehicle" => new VehicleProductFactory(),
                _ => null
            };

            if (factory != null)
            {
                var newProduct = factory.CreateProduct(name, price);
                newProduct.Stock = stock ?? 10;

                newProduct.AvailableColors = availableColors;

                if (newProduct is ElectronicProduct electronic)
                {
                    if (warrantyMonths.HasValue) electronic.WarrantyMonths = warrantyMonths.Value;
                }
                else if (newProduct is ClothingProduct clothing)
                {
                    if (!string.IsNullOrEmpty(size)) clothing.Size = size;
                    if (!string.IsNullOrEmpty(material)) clothing.Material = material;
                    clothing.AvailableSizes = availableSizes;
                }
                else if (newProduct is VehicleProduct vehicle)
                {
                    if (!string.IsNullOrEmpty(brand)) vehicle.Brand = brand;
                    if (!string.IsNullOrEmpty(model)) vehicle.Model = model;
                    if (year.HasValue) vehicle.Year = year.Value;
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
                                IsMain = (i == 0),
                                DisplayOrder = i
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
        public IActionResult EditProduct(Guid id)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Guid id, string name, decimal price, int? stock, int? warrantyMonths, string size, string material, string availableSizes, string availableColors, string brand, string model, int? year, List<IFormFile>? newImages, List<string>? deletePublicIds)
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

            if (product is ElectronicProduct electroUpdate)
            {
                if (warrantyMonths.HasValue) electroUpdate.WarrantyMonths = warrantyMonths.Value;
            }
            else if (product is ClothingProduct clothingUpdate)
            {
                clothingUpdate.Size = size;
                clothingUpdate.Material = material;
                clothingUpdate.AvailableSizes = availableSizes;
            }
            else if (product is VehicleProduct vehicleUpdate)
            {
                vehicleUpdate.Brand = brand;
                vehicleUpdate.Model = model;
                if (year.HasValue) vehicleUpdate.Year = year.Value;
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
                                IsMain = (existingCount == 0 && i == 0),
                                DisplayOrder = existingCount + i
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
                ElectronicProduct ep => ep.Clone(),
                ClothingProduct cp => cp.Clone(),
                VehicleProduct vp => vp.Clone(),
                _ => throw new NotSupportedException("Product type not supported for cloning.")
            };

            _productRepo.Add(clone);
            return RedirectToAction("Products");
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

                TempData["Success"] = $"Statusul comenzii {orderId} a fost actualizat la {status}.";
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
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Factories;
using System;
using System.Linq;
using System.Collections.Generic;

namespace OnlineStore.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly DbProductRepository _productRepo;
        private readonly DbOrderRepository _orderRepo;
        private readonly DbUserRepository _userRepo;

        public AdminController(DbProductRepository productRepo, DbOrderRepository orderRepo, DbUserRepository userRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
        }

        private bool IsAdmin()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return false;

            if (Guid.TryParse(userIdStr, out Guid userId))
            {
                var user = _userRepo.GetById(userId);
                return user is Admin;
            }
            return false;
        }

        private IActionResult CheckAccess()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        public IActionResult Index()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

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
        public IActionResult AddProduct(string type, string name, decimal price, int? warrantyMonths, string size, string material, string brand, string model, int? year)
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

                // Override defaults if specific values provided in form
                if (newProduct is ElectronicProduct electronic)
                {
                    if (warrantyMonths.HasValue) electronic.WarrantyMonths = warrantyMonths.Value;
                }
                else if (newProduct is ClothingProduct clothing)
                {
                    if (!string.IsNullOrEmpty(size)) clothing.Size = size;
                    if (!string.IsNullOrEmpty(material)) clothing.Material = material;
                }
                else if (newProduct is VehicleProduct vehicle)
                {
                    if (!string.IsNullOrEmpty(brand)) vehicle.Brand = brand;
                    if (!string.IsNullOrEmpty(model)) vehicle.Model = model;
                    if (year.HasValue) vehicle.Year = year.Value;
                }

                _productRepo.Add(newProduct);
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
        public IActionResult EditProduct(Guid id, string name, decimal price, int? warrantyMonths, string size, string material, string brand, string model, int? year)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            product.Name = name;
            product.Price = price;

            if (product is ElectronicProduct electroUpdate)
            {
                if (warrantyMonths.HasValue) electroUpdate.WarrantyMonths = warrantyMonths.Value;
            }
            else if (product is ClothingProduct clothingUpdate)
            {
                clothingUpdate.Size = size;
                clothingUpdate.Material = material;
            }
            else if (product is VehicleProduct vehicleUpdate)
            {
                vehicleUpdate.Brand = brand;
                vehicleUpdate.Model = model;
                if (year.HasValue) vehicleUpdate.Year = year.Value;
            }

            _productRepo.Update(product);
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
    }
}

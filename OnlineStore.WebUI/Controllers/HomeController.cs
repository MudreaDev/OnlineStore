using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Singleton;
using OnlineStore.WebUI.Models;

namespace OnlineStore.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbProductRepository _productRepo;

        public HomeController(ILogger<HomeController> logger, DbProductRepository productRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
        }

        public IActionResult Index(string? searchQuery, string? categoryFilter, decimal? minPrice, decimal? maxPrice)
        {
            var products = _productRepo.GetAll();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "All")
            {
                products = products.Where(p => p.GetType().Name.StartsWith(categoryFilter));
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Persistence for UI
            ViewBag.SearchQuery = searchQuery;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // Pattern 3: Singleton - Transmitem setările globale către View
            ViewBag.FreeShippingThreshold = ApplicationConfigurationManager.Instance.FreeShippingThreshold;
            ViewBag.CurrencySymbol = ApplicationConfigurationManager.Instance.CurrencySymbol;

            return View(products.ToList());
        }

        public IActionResult Details(Guid id)
        {
            var product = _productRepo.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Singleton;
using OnlineStore.WebUI.Models;
using OnlineStore.Domain.DesignPatterns.Structural.Composite;
using OnlineStore.Domain.DesignPatterns.Behavioral.Iterator;
using OnlineStore.Domain.Strategies;

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

        public IActionResult Index(string? searchQuery, string? categoryFilter, decimal? minPrice, decimal? maxPrice, string? sortStrategy)
        {
            var allProducts = _productRepo.GetAll().ToList();
            var filteredProducts = new List<Product>();

            // Pattern: Iterator - utilizăm Iterator pentru a parcurge și filtra produsele după categorie
            IProductCollection productCollection = new ProductCollection(allProducts);
            IProductIterator iterator = productCollection.CreateIterator(categoryFilter);

            while (iterator.HasNext())
            {
                filteredProducts.Add(iterator.Next());
            }

            var productsQuery = filteredProducts.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            IEnumerable<Product> finalProducts = productsQuery;

            // Pattern: Strategy extins pentru sortare
            ISortingStrategy strategy = sortStrategy switch
            {
                "PriceAsc" => new PriceAscendingStrategy(),
                "PriceDesc" => new PriceDescendingStrategy(),
                "Name" => new NameSortingStrategy(),
                "Stock" => new StockSortingStrategy(),
                _ => null!
            };

            if (strategy != null)
            {
                finalProducts = strategy.Sort(finalProducts);
            }

            // Persistence for UI
            ViewBag.SearchQuery = searchQuery;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            ViewBag.SortStrategy = sortStrategy;

            // Pattern 3: Singleton - Transmitem setările globale către View
            ViewBag.FreeShippingThreshold = ApplicationConfigurationManager.Instance.FreeShippingThreshold;
            ViewBag.CurrencySymbol = ApplicationConfigurationManager.Instance.CurrencySymbol;

            return View(finalProducts.ToList());
        }

        public IActionResult Catalog()
        {
            var products = _productRepo.GetAll().ToList();

            var mainCatalog = new ProductCategory("Catalog Produse (Composite Pattern)");

            var groups = products.GroupBy(p => p.GetType().Name.Replace("Product", ""));

            foreach (var group in groups)
            {
                var category = new ProductCategory(group.Key);
                foreach (var p in group)
                {
                    var mainImage = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl ?? "";
                    category.Add(new ProductItem(p.Name, p.Price, p.Id, mainImage));
                }
                mainCatalog.Add(category);
            }

            return View(mainCatalog);
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

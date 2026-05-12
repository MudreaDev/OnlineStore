using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Application.Data;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Singleton;
using OnlineStore.WebUI.Models;
using OnlineStore.Domain.DesignPatterns.Structural.Composite;
using OnlineStore.Domain.DesignPatterns.Structural.Flyweight;
using OnlineStore.Domain.DesignPatterns.Behavioral.Iterator;
using OnlineStore.Domain.Strategies;
using OnlineStore.Application.Services;

namespace OnlineStore.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbProductRepository _productRepo;
        private readonly ReviewService _reviewService;
        private readonly ToastService _toastService;
        private readonly CurrencyService _currencyService;
        private readonly OnlineStoreDbContext _context;

        public HomeController(ILogger<HomeController> logger, DbProductRepository productRepo, OnlineStoreDbContext context, CurrencyService currencyService, ReviewService reviewService, ToastService toastService)
        {
            _logger = logger;
            _productRepo = productRepo;
            _context = context;
            _currencyService = currencyService;
            _reviewService = reviewService;
            _toastService = toastService;
        }

        public IActionResult Index(string? searchQuery, string? categoryFilter, decimal? minPrice, decimal? maxPrice, string? sortStrategy, string[]? selectedSizes, string[]? selectedColors, string[]? selectedFuelTypes, int? minYear, int? maxYear, int page = 1)
        {
            int pageSize = 25; // 5x5 grid
            var allProducts = _productRepo.GetAll().ToList();
            var filteredProducts = new List<Product>();

            // Pattern: Iterator - utilizăm Iterator pentru a parcurge și filtra produsele după categorie
            IProductCollection productCollection = new ProductCollection(allProducts);
            IProductIterator iterator = productCollection.CreateIterator(categoryFilter);

            while (iterator.HasNext())
            {
                filteredProducts.Add(iterator.Next());
            }

            var productsQuery = filteredProducts.AsEnumerable();

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

            // Filter by sizes
            if (selectedSizes != null && selectedSizes.Any())
            {
                productsQuery = productsQuery.Where(p => 
                    p is ClothingProduct cp && !string.IsNullOrEmpty(cp.AvailableSizes) &&
                    selectedSizes.Any(s => cp.AvailableSizes.Split(',', StringSplitOptions.TrimEntries)
                        .Contains(s, StringComparer.OrdinalIgnoreCase)));
            }

            // Filter by colors
            if (selectedColors != null && selectedColors.Any())
            {
                var colorMap = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Black", new[] { "Black", "#000000", "#000", "Noir" } },
                    { "White", new[] { "White", "#FFFFFF", "#fff", "Blanc" } },
                    { "Red", new[] { "Red", "#FF0000", "#f00", "Rouge" } },
                    { "Blue", new[] { "Blue", "#0000FF", "#00f", "Bleu" } },
                    { "Beige", new[] { "Beige", "#F5F5DC", "#f5f5dc" } },
                    { "Navy", new[] { "Navy", "#000080", "#000080" } },
                    { "Charcoal", new[] { "Charcoal", "#36454F", "#36454f" } },
                    { "Sage", new[] { "Sage", "#BCB88A", "#bcb88a" } }
                };

                productsQuery = productsQuery.Where(p => 
                    !string.IsNullOrEmpty(p.AvailableColors) &&
                    selectedColors.Any(c => {
                        var searchValues = colorMap.TryGetValue(c, out var values) ? values : new[] { c };
                        var productColors = p.AvailableColors.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        return searchValues.Any(sv => productColors.Contains(sv, StringComparer.OrdinalIgnoreCase));
                    }));
            }

            // Vehicle filters
            if (selectedFuelTypes != null && selectedFuelTypes.Any())
            {
                productsQuery = productsQuery.Where(p => 
                    p is VehicleProduct vp && !string.IsNullOrEmpty(vp.FuelType) &&
                    selectedFuelTypes.Contains(vp.FuelType, StringComparer.OrdinalIgnoreCase));
            }

            if (minYear.HasValue)
            {
                productsQuery = productsQuery.Where(p => p is VehicleProduct vp && vp.Year >= minYear.Value);
            }

            if (maxYear.HasValue)
            {
                productsQuery = productsQuery.Where(p => p is VehicleProduct vp && vp.Year <= maxYear.Value);
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

            // Filtrare: doar produse în stoc pe pagina principală
            finalProducts = finalProducts.Where(p => p.Stock > 0);

            var totalItems = finalProducts.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

            var finalList = finalProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Pattern: Flyweight — resetăm statisticile la fiecare request pentru o demonstrație corectă în banner
            ProductTypeFlyweightFactory.Reset();

            // Pattern: Flyweight — refolosim instanțe partajate pentru metadatele de tip
            foreach (var p in finalList)
            {
                var typeName = p.GetType().Name.Replace("Product", "");
                var fw = ProductTypeFlyweightFactory.GetFlyweight(typeName);
                // flyweight-ul e folosit: starea extrinsecă e p.Name
                var _ = fw.RenderBadge(p.Name);
            }
            ViewBag.FlyweightTotalProducts = finalList.Count;
            ViewBag.FlyweightCacheSize = ProductTypeFlyweightFactory.CacheSize;
            ViewBag.FlyweightTotalRequests = ProductTypeFlyweightFactory.TotalRequests;

            // Persistence for UI
            ViewBag.SearchQuery = searchQuery;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortStrategy = sortStrategy;
            ViewBag.SelectedSizes = selectedSizes;
            ViewBag.SelectedColors = selectedColors;
            ViewBag.SelectedFuelTypes = selectedFuelTypes;
            ViewBag.MinYear = minYear;
            ViewBag.MaxYear = maxYear;

            // Pattern 3: Singleton - Transmitem setările globale către View
            ViewBag.FreeShippingThreshold = ApplicationConfigurationManager.Instance.FreeShippingThreshold;
            ViewBag.CurrencySymbol = ApplicationConfigurationManager.Instance.CurrencySymbol;

            // Dynamic Categories for Hero Banners
            ViewBag.Categories = _context.Categories.ToList();

            return View(finalList);
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

        public IActionResult OutOfStock()
        {
            var outOfStockProducts = _productRepo.GetAll().Where(p => p.Stock <= 0).ToList();
            return View(outOfStockProducts);
        }

        public IActionResult Details(Guid id)
        {
            var product = _productRepo.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            // Pattern: Strategy pentru recomandări
            IRecommendationStrategy recommendationStrategy = new SameCategoryRecommendationStrategy();
            var allProducts = _productRepo.GetAll();
            var recommended = recommendationStrategy.GetRecommendations(product, allProducts, 5);
            
            ViewBag.Recommendations = recommended;

            // Recenzii (Composite Pattern)
            var reviews = _reviewService.GetProductReviews(id);
            ViewBag.Reviews = reviews;

            return View(product);
        }

        [HttpPost]
        public IActionResult AddReview(Guid productId, string content, int rating, Guid? parentId)
        {
            var username = HttpContext.Session.GetString("Username") ?? "Anonymous";
            var review = new ProductReview
            {
                ProductId = productId,
                UserName = username,
                Content = content,
                Rating = rating,
                ParentReviewId = parentId
            };

            _reviewService.AddReview(review);
            _toastService.AddToast("Recenzia a fost adăugată cu succes!");
            return RedirectToAction("Details", new { id = productId });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetCurrency(string currencyCode, string returnUrl)
        {
            _currencyService.SetCurrency(currencyCode);
            _toastService.AddToast($"Moneda a fost schimbată în {currencyCode}.", "info");
            return LocalRedirect(returnUrl ?? "/");
        }

        public IActionResult Error404()
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

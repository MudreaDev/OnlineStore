using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.WebUI.Models;

namespace OnlineStore.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InMemoryProductRepository _productRepo;

        public HomeController(ILogger<HomeController> logger, InMemoryProductRepository productRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
        }

        public IActionResult Index()
        {
            var products = _productRepo.GetAll();
            return View(products);
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

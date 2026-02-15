using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.WebUI.Models;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Factories;

namespace OnlineStore.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly ElectronicProductFactory _electronicFactory;
    private readonly ClothingProductFactory _clothingFactory;

    public HomeController(ElectronicProductFactory electronicFactory, ClothingProductFactory clothingFactory)
    {
        _electronicFactory = electronicFactory;
        _clothingFactory = clothingFactory;
    }

    public IActionResult Index()
    {
        var products = new List<Product>
        {
            _electronicFactory.CreateProduct("Smartphone Pro", 999.99m),
            _clothingFactory.CreateProduct("Designer Jeans", 79.50m),
            _electronicFactory.CreateProduct("Wireless Earbuds", 149.00m),
            _clothingFactory.CreateProduct("Wool Sweater", 55.00m),
            _electronicFactory.CreateProduct("4K Monitor", 349.99m),
            _clothingFactory.CreateProduct("Running Shoes", 120.00m)
        };

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

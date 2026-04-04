using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.WebUI.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.WebUI.Controllers
{
    public class WishlistController : Controller
    {
        private readonly DbProductRepository _productRepo;
        private const string SessionKey = "Wishlist";

        public WishlistController(DbProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public IActionResult Index()
        {
            var wishlist = GetWishlist();
            var products = wishlist.Select(id => _productRepo.GetById(id)).Where(p => p != null).ToList();
            return View(products);
        }

        [HttpPost]
        public IActionResult Toggle(Guid id)
        {
            var wishlist = GetWishlist();
            bool added = false;

            if (wishlist.Contains(id))
            {
                wishlist.Remove(id);
            }
            else
            {
                wishlist.Add(id);
                added = true;
            }

            SaveWishlist(wishlist);

            return Json(new { success = true, added = added, count = wishlist.Count });
        }

        [HttpGet]
        public IActionResult GetCount()
        {
            var wishlist = GetWishlist();
            return Json(new { count = wishlist.Count });
        }

        private List<Guid> GetWishlist()
        {
            return HttpContext.Session.Get<List<Guid>>(SessionKey) ?? new List<Guid>();
        }

        private void SaveWishlist(List<Guid> wishlist)
        {
            HttpContext.Session.Set(SessionKey, wishlist);
        }
    }
}

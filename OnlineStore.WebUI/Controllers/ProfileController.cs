using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using System;
using System.Linq;

namespace OnlineStore.WebUI.Controllers
{
    public class ProfileController : Controller
    {
        private readonly DbUserRepository _userRepo;

        public ProfileController(DbUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = Guid.Parse(userIdStr);
            var user = _userRepo.GetById(userId);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(string username, string email, string shippingAddress)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = Guid.Parse(userIdStr);
            var user = _userRepo.GetById(userId);

            if (user == null) return NotFound();

            user.Username = username;
            user.Email = email;

            if (user is Customer customer)
            {
                customer.ShippingAddress = shippingAddress;
            }

            _userRepo.Update(user);

            HttpContext.Session.SetString("Username", user.Username);
            TempData["Success"] = "Profilul a fost actualizat.";

            return RedirectToAction("Index");
        }
    }
}

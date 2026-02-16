using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.WebUI.Extensions;
using OnlineStore.WebUI.Models;

namespace OnlineStore.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbUserRepository _userRepo;

        public AccountController(DbUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string email)
        {
            // Simple logic: Find or Create
            var user = _userRepo.GetAll().FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                user = new Customer(username, email, "Default Address");
                _userRepo.Add(user);
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if username already exists
            var existingUser = _userRepo.GetAll().FirstOrDefault(u => u.Username == model.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(model);
            }

            // Create new Customer
            var customer = new Customer(model.Username, model.Email, model.ShippingAddress);
            _userRepo.Add(customer);

            // Log in the new user
            HttpContext.Session.SetString("UserId", customer.Id.ToString());
            HttpContext.Session.SetString("Username", customer.Username);

            return RedirectToAction("Index", "Home");
        }
    }
}

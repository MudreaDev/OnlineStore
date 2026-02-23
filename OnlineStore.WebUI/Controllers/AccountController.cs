using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Utils;
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
        public IActionResult Login(string username, string password)
        {
            var user = _userRepo.GetAll().FirstOrDefault(u => u.Username == username);

            if (user != null && PasswordHasher.Verify(password, user.PasswordHash))
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                TempData["Success"] = "Autentificare reușită!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Nume de utilizator sau parolă incorectă.");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "V-ați deconectat cu succes.";
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
                ModelState.AddModelError("Username", "Utilizatorul există deja.");
                return View(model);
            }

            // Create new Customer
            string hashedPassword = PasswordHasher.Hash(model.Password);
            var customer = new Customer(model.Username, model.Email, model.ShippingAddress);
            customer.PasswordHash = hashedPassword;

            _userRepo.Add(customer);

            // Log in the new user
            HttpContext.Session.SetString("UserId", customer.Id.ToString());
            HttpContext.Session.SetString("Username", customer.Username);
            TempData["Success"] = "Cont creat cu succes!";

            return RedirectToAction("Index", "Home");
        }
    }
}

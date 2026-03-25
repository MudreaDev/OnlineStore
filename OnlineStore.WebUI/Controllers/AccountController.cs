using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Utils;
using OnlineStore.WebUI.Models;
using OnlineStore.Domain.Interfaces;
using System.Threading.Tasks;

namespace OnlineStore.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbUserRepository _userRepo;
        private readonly IEmailService _emailService;

        public AccountController(DbUserRepository userRepo, IEmailService emailService)
        {
            _userRepo = userRepo;
            _emailService = emailService;
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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _userRepo.GetAll().FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                // For security reasons, don't reveal if the user exists or not
                TempData["Success"] = "Dacă adresa de email există în sistemul nostru, vei primi un cod de resetare.";
                return RedirectToAction("Login");
            }

            // Generate 6-digit OTP
            var code = new Random().Next(100000, 999999).ToString();
            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiration = DateTime.Now.AddMinutes(3);

            _userRepo.Update(user);

            await _emailService.SendPasswordResetCodeAsync(user.Email, code);

            TempData["Success"] = "Codul de resetare a fost trimis pe email.";
            return RedirectToAction("ResetPassword", new { email = model.Email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            return View(new ResetPasswordViewModel { Email = email });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _userRepo.GetAll().FirstOrDefault(u => u.Email == model.Email);

            if (user == null || user.PasswordResetCode != model.Code || user.PasswordResetCodeExpiration < DateTime.Now)
            {
                ModelState.AddModelError("", "Cod invalid sau expirat.");
                return View(model);
            }

            // Update password
            user.PasswordHash = PasswordHasher.Hash(model.NewPassword);
            user.PasswordResetCode = null;
            user.PasswordResetCodeExpiration = null;

            _userRepo.Update(user);

            TempData["Success"] = "Parola a fost resetată cu succes. Te poți autentifica.";
            return RedirectToAction("Login");
        }
    }
}

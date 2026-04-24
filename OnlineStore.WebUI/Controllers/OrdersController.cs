using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Application.Patterns.TemplateMethod;
using System;
using System.Linq;
using System.Text;

namespace OnlineStore.WebUI.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DbOrderRepository _orderRepo;
        private readonly DbUserRepository _userRepo;

        public OrdersController(DbOrderRepository orderRepo, DbUserRepository userRepo)
        {
            _orderRepo = orderRepo;
            _userRepo = userRepo;
        }

        private Guid? GetUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return null;
            return Guid.Parse(userIdStr);
        }

        public IActionResult MyOrders()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var orders = _orderRepo.GetAll()
                .Where(o => o.User.Id == userId.Value)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        public IActionResult OrderDetails(Guid id)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var order = _orderRepo.GetById(id);
            if (order == null || order.User.Id != userId.Value)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public IActionResult DownloadInvoice(Guid id)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var order = _orderRepo.GetById(id);
            if (order == null || order.User.Id != userId.Value)
            {
                return NotFound();
            }

            var generator = new InvoiceGenerator();
            string invoiceContent = generator.Generate(order);

            var fileName = $"Invoice_{order.Id.ToString()[..8]}.txt";
            return File(Encoding.UTF8.GetBytes(invoiceContent), "text/plain", fileName);
        }
    }
}

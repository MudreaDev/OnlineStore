using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using System;
using System.Linq;

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
    }
}

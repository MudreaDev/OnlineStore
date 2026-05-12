using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Data;
using System;
using System.Linq;

namespace OnlineStore.WebUI.Controllers
{
    public class NotificationController : Controller
    {
        private readonly OnlineStoreDbContext _context;

        public NotificationController(OnlineStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetLatest()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return Json(new { count = 0 });

            var userId = Guid.Parse(userIdStr);
            var notifications = _context.UserNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderBy(n => n.CreatedAt)
                .ToList();

            foreach (var n in notifications)
            {
                n.IsRead = true;
            }
            _context.SaveChanges();

            return Json(notifications);
        }
    }
}

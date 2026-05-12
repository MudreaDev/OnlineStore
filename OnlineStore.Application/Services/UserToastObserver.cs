using System;
using OnlineStore.Domain.DesignPatterns.Behavioral.Observer;
using OnlineStore.Domain.Entities;
using OnlineStore.Application.Data;

namespace OnlineStore.Application.Services
{
    public class UserToastObserver : IOrderObserver
    {
        private readonly OnlineStoreDbContext _context;

        public UserToastObserver(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public void Update(Order order)
        {
            // Verificăm dacă avem ID-ul utilizatorului (fie prin navigare, fie prin FK)
            var userId = order.User?.Id ?? order.UserId;
            
            if (userId == Guid.Empty) return;

            // Adaugă o notificare în baza de date pentru utilizator
            var notification = new UserNotification(
                userId,
                $"Statusul comenzii tale #{order.Id.ToString().Substring(0,8)} a fost actualizat la: {order.Status}.",
                "info"
            );

            _context.UserNotifications.Add(notification);
            _context.SaveChanges();
        }
    }
}

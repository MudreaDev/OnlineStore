using OnlineStore.Application.Data;
using OnlineStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Application.Services
{
    public class ReviewService
    {
        private readonly OnlineStoreDbContext _context;
        private readonly ReviewNotificationService _notificationService;

        public ReviewService(OnlineStoreDbContext context, ReviewNotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
            
            // Atașăm observatorul de alerte admin
            _notificationService.Attach(new AdminAlertObserver());
        }

        public void AddReview(ProductReview review)
        {
            _context.ProductReviews.Add(review);
            _context.SaveChanges();
            
            // Notificăm observatorii (Observer Pattern)
            _notificationService.Notify(review);
        }

        public List<ProductReview> GetProductReviews(Guid productId)
        {
            // Preluăm doar recenziile rădăcină (fără părinte)
            // Composite Pattern: Copiii vor fi încărcați prin proprietatea Replies
            return _context.ProductReviews
                .Include(r => r.Replies)
                .Where(r => r.ProductId == productId && r.ParentReviewId == null)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }
    }
}

using OnlineStore.Domain.Entities;
using System.Collections.Generic;

namespace OnlineStore.Application.Services
{
    /// <summary>
    /// Interfața pentru observatorii de recenzii.
    /// </summary>
    public interface IReviewObserver
    {
        void Update(ProductReview review);
    }

    /// <summary>
    /// Subject pentru Recenzii (Observer Pattern).
    /// </summary>
    public class ReviewNotificationService
    {
        private readonly List<IReviewObserver> _observers = new List<IReviewObserver>();

        public void Attach(IReviewObserver observer) => _observers.Add(observer);
        
        public void Notify(ProductReview review)
        {
            foreach (var observer in _observers)
            {
                observer.Update(review);
            }
        }
    }

    /// <summary>
    /// Un observator concret care loghează recenziile negative pentru admin.
    /// </summary>
    public class AdminAlertObserver : IReviewObserver
    {
        public void Update(ProductReview review)
        {
            if (review.Rating <= 2)
            {
                // În realitate, am trimite un email sau am pune o alertă în dashboard
                System.Diagnostics.Debug.WriteLine($"[ALERT ADMIN] Recenzie negativă de la {review.UserName}: {review.Content}");
            }
        }
    }
}

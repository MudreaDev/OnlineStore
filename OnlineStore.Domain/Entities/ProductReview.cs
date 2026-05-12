using System;
using System.Collections.Generic;

namespace OnlineStore.Domain.Entities
{
    /// <summary>
    /// Entitate pentru Recenzii. 
    /// Implementează o structură de tip Composite prin ParentReviewId, permițând răspunsuri la recenzii.
    /// </summary>
    public class ProductReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; } // 1-5
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Composite Pattern: O recenzie poate fi un răspuns la altă recenzie
        public Guid? ParentReviewId { get; set; }
        public virtual ICollection<ProductReview> Replies { get; set; } = new List<ProductReview>();
    }
}

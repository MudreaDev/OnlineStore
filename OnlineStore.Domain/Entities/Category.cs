using OnlineStore.Domain.Common;
using System.Collections.Generic;

namespace OnlineStore.Domain.Entities
{
    public class Category : Entity
    {
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        public bool IsFeatured { get; set; } = true;
        public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    }
}

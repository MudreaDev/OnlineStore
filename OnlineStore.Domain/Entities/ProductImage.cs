using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities
{
    public class ProductImage : Entity
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string PublicId { get; set; } = null!;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }

        // Navigation property
        public Product? Product { get; set; }

        // Parameterless constructor for EF Core
        public ProductImage() : base()
        {
        }
    }
}

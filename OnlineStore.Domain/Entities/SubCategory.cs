using OnlineStore.Domain.Common;
using System;
using System.Collections.Generic;

namespace OnlineStore.Domain.Entities
{
    public class SubCategory : Entity
    {
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string Name { get; set; } = null!;
        
        // Stochează atributele cerute pentru acest tip de produs. Ex: "RAM,Procesor,Baterie"
        public string ExpectedAttributes { get; set; } = string.Empty;
        
        public List<Product> Products { get; set; } = new List<Product>();
    }
}

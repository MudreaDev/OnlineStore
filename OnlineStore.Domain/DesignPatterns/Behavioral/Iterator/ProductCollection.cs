using System.Collections.Generic;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Iterator
{
    public class ProductCollection : IProductCollection
    {
        private readonly List<Product> _products = new List<Product>();

        public ProductCollection(IEnumerable<Product> products)
        {
            _products.AddRange(products);
        }

        public IProductIterator CreateIterator(string? typeFilter)
        {
            return new ProductTypeIterator(_products, typeFilter);
        }
    }
}

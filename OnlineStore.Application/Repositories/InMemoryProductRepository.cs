using System;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Repositories
{
    public class InMemoryProductRepository : IReadableRepository<Product>, IWriteableRepository<Product>
    {
        private readonly List<Product> _products = new List<Product>();

        public void Add(Product entity)
        {
            _products.Add(entity);
        }

        public void Delete(Guid id)
        {
            var product = GetById(id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }

        public IEnumerable<Product> GetAll()
        {
            return _products;
        }

        public Product GetById(Guid id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public void Update(Product entity)
        {
            var existingProduct = GetById(entity.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = entity.Name;
                existingProduct.Price = entity.Price;

            }
        }
    }
}

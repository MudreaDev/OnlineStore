using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace OnlineStore.Application.Services
{
    public interface IStockService
    {
        bool IsInStock(Guid productId, int quantity);
        void UpdateStock(Guid productId, int quantityChange);
    }

    public class StockService : IStockService
    {
        private readonly IReadableRepository<Product> _productRepo;
        private readonly IWriteableRepository<Product> _writeableRepo;

        public StockService(IReadableRepository<Product> productRepo, IWriteableRepository<Product> writeableRepo)
        {
            _productRepo = productRepo;
            _writeableRepo = writeableRepo;
        }

        public bool IsInStock(Guid productId, int quantity)
        {
            var product = _productRepo.GetById(productId);
            return product != null && product.Stock >= quantity;
        }

        public void UpdateStock(Guid productId, int quantityChange)
        {
            var product = _productRepo.GetById(productId);
            if (product != null)
            {
                product.Stock += quantityChange;
                _writeableRepo.Update(product);
                Console.WriteLine($"Stock updated for {product.Name}. New stock: {product.Stock}");
            }
        }
    }
}

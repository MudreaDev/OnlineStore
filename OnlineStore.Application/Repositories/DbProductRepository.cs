using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Application.Data;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Repositories
{
    public class DbProductRepository : IReadableRepository<Product>, IWriteableRepository<Product>
    {
        private readonly OnlineStoreDbContext _context;

        public DbProductRepository(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public void Add(Product entity)
        {
            _context.Products.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.Include(p => p.Images).ToList();
        }

        public Product GetById(Guid id)
        {
            return _context.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == id);
        }

        public void Update(Product entity)
        {
            _context.Products.Update(entity);
            _context.SaveChanges();
        }
    }
}

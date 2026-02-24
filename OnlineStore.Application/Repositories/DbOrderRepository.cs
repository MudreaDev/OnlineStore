using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Application.Data;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Repositories
{
    public class DbOrderRepository : IReadableRepository<Order>, IWriteableRepository<Order>
    {
        private readonly OnlineStoreDbContext _context;

        public DbOrderRepository(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public void Add(Order entity)
        {
            // Ensure User is attached if necessary, but assume entity is valid
            // If User is new, EF will Insert it. If it exists, we might need to attach.
            // For now, assuming standard add.
            _context.Orders.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Order> GetAll()
        {
            // Include related data
            return _context.Orders
                .Include(o => o.Items)
                .Include(o => o.User)
                .ToList();
        }

        public Order GetById(Guid id)
        {
            return _context.Orders
                .Include(o => o.Items)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);
        }

        public void Update(Order entity)
        {
            _context.Orders.Update(entity);
            _context.SaveChanges();
        }
    }
}

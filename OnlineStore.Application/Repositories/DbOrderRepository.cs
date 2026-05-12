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
            // Păstrăm referința pentru a o restaura ulterior
            var userRef = entity.User;
            
            // Setăm proprietatea de navigare pe null pentru a forța EF să folosească doar UserId
            // Acest lucru previne eroarea de tip "An error occurred while saving the entity changes" 
            // cauzată de încercarea de a re-insera un utilizator care există deja în DB.
            entity.User = null!;
            
            _context.Orders.Add(entity);
            _context.SaveChanges();
            
            // Restaurăm referința pentru a fi disponibilă în obiectul returnat
            entity.User = userRef;
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

        public Order? GetById(Guid id)
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

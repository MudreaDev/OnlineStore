using System;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Repositories
{
    public class InMemoryOrderRepository : IReadableRepository<Order>, IWriteableRepository<Order>
    {
        private readonly List<Order> _orders = new List<Order>();

        public void Add(Order entity)
        {
            _orders.Add(entity);
        }

        public void Delete(Guid id)
        {
            var order = GetById(id);
            if (order != null)
            {
                _orders.Remove(order);
            }
        }

        public IEnumerable<Order> GetAll()
        {
            return _orders;
        }

        public Order GetById(Guid id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }

        public void Update(Order entity)
        {
            var existingOrder = GetById(entity.Id);
            if (existingOrder != null)
            {
                existingOrder.Status = entity.Status;
                existingOrder.Total = entity.Total;
                // Update other properties
            }
        }
    }
}

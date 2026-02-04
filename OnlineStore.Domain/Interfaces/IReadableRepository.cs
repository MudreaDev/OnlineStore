using OnlineStore.Domain.Entities;
using System.Collections.Generic;

namespace OnlineStore.Domain.Interfaces
{
    // ISP: Interface Segregation Principle
    public interface IReadableRepository<T> where T : class
    {
        T GetById(Guid id);
        IEnumerable<T> GetAll();
    }
}
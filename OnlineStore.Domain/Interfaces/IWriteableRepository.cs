using OnlineStore.Domain.Entities;
using System.Collections.Generic;

namespace OnlineStore.Domain.Interfaces
{
  

    public interface IWriteableRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(Guid id);
    }
}
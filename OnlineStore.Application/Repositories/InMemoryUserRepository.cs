using System;
using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Repositories
{
    public class InMemoryUserRepository : IReadableRepository<User>, IWriteableRepository<User>
    {
        private readonly List<User> _users = new List<User>();

        public void Add(User entity)
        {
            _users.Add(entity);
        }

        public void Delete(Guid id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _users.Remove(user);
            }
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User GetById(Guid id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public void Update(User entity)
        {
            var existingUser = GetById(entity.Id);
            if (existingUser != null)
            {
                existingUser.Username = entity.Username;
                existingUser.Email = entity.Email;
                // Update other properties if necessary
            }
        }
    }
}

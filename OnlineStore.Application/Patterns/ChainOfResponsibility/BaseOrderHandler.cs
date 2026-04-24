using OnlineStore.Domain.Entities;
using System.Collections.Generic;

namespace OnlineStore.Application.Patterns.ChainOfResponsibility
{
    public abstract class BaseOrderHandler : IOrderHandler
    {
        private IOrderHandler? _nextHandler;

        public IOrderHandler SetNext(IOrderHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual string? Handle(User user, List<OrderItem> items)
        {
            if (_nextHandler != null)
            {
                return _nextHandler.Handle(user, items);
            }

            return null;
        }
    }
}

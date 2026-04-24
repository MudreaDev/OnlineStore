using OnlineStore.Domain.Entities;
using System.Collections.Generic;

namespace OnlineStore.Application.Patterns.ChainOfResponsibility
{
    public interface IOrderHandler
    {
        IOrderHandler SetNext(IOrderHandler handler);
        string? Handle(User user, List<OrderItem> items);
    }
}

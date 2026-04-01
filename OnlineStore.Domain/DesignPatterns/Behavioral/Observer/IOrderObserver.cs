using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Observer
{
    public interface IOrderObserver
    {
        void Update(Order order);
    }
}

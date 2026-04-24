using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Patterns.State
{
    public interface IOrderState
    {
        void Pay(Order order);
        void Ship(Order order);
        void Cancel(Order order);
        string GetStatusName();
    }
}

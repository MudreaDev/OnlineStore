using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Iterator
{
    public interface IProductIterator
    {
        bool HasNext();
        Product Next();
        void Reset();
    }
}

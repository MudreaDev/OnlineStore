using System.Collections.Generic;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Strategies
{
    public interface ISortingStrategy
    {
        IEnumerable<Product> Sort(IEnumerable<Product> products);
    }
}

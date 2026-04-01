using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Strategies
{
    public class PriceDescendingStrategy : ISortingStrategy
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderByDescending(p => p.Price);
        }
    }
}

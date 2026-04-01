using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Strategies
{
    public class NameSortingStrategy : ISortingStrategy
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderBy(p => p.Name);
        }
    }
}

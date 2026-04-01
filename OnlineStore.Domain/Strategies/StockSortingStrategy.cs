using System.Collections.Generic;
using System.Linq;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Strategies
{
    public class StockSortingStrategy : ISortingStrategy
    {
        public IEnumerable<Product> Sort(IEnumerable<Product> products)
        {
            return products.OrderByDescending(p => p.Stock); // Cele cu mai mult stock primele
        }
    }
}

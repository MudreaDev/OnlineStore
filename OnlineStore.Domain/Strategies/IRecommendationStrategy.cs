using OnlineStore.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Domain.Strategies
{
    /// <summary>
    /// Pattern Strategy pentru recomandări de produse.
    /// Permite schimbarea algoritmului de recomandare fără a modifica codul apelant.
    /// </summary>
    public interface IRecommendationStrategy
    {
        IEnumerable<Product> GetRecommendations(Product currentProduct, IEnumerable<Product> allProducts, int count);
    }

    public class SameCategoryRecommendationStrategy : IRecommendationStrategy
    {
        public IEnumerable<Product> GetRecommendations(Product currentProduct, IEnumerable<Product> allProducts, int count)
        {
            string category = currentProduct.GetType().Name;
            return allProducts
                .Where(p => p.Id != currentProduct.Id && p.GetType().Name == category)
                .Take(count);
        }
    }

    public class RandomRecommendationStrategy : IRecommendationStrategy
    {
        public IEnumerable<Product> GetRecommendations(Product currentProduct, IEnumerable<Product> allProducts, int count)
        {
            return allProducts
                .Where(p => p.Id != currentProduct.Id)
                .OrderBy(x => System.Guid.NewGuid())
                .Take(count);
        }
    }
}

using System.Collections.Generic;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Iterator
{
    /// <summary>
    /// Iterator concret cu filtrare după tip de produs.
    /// HasNext() și Next() sunt sincronizate corect conform Refactoring Guru.
    /// </summary>
    public class ProductTypeIterator : IProductIterator
    {
        private readonly List<Product> _products;
        private readonly string? _typeFilter;
        private int _position = 0; // start de la 0
        private int _nextIndex = -1; // Indexul următorului element valid găsite de HasNext

        public ProductTypeIterator(List<Product> products, string? typeFilter)
        {
            _products = products;
            _typeFilter = typeFilter;
        }

        public bool HasNext()
        {
            if (_nextIndex != -1) return true;

            // Caută de la poziția curentă înainte
            for (int i = _position; i < _products.Count; i++)
            {
                if (Matches(i))
                {
                    _nextIndex = i;
                    return true;
                }
            }
            return false;
        }

        public Product Next()
        {
            if (!HasNext()) return null!;

            _position = _nextIndex;
            var product = _products[_position];
            _position++;
            _nextIndex = -1; // Resetăm pentru următorul apel HasNext
            return product;
        }

        public void Reset()
        {
            _position = 0;
            _nextIndex = -1;
        }

        private bool Matches(int index)
        {
            if (index >= _products.Count) return false;
            if (string.IsNullOrEmpty(_typeFilter) || _typeFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
                return true;

            var product = _products[index];
            var typeName = product.GetType().Name;
            var categoryName = product.SubCategory?.Category?.Name ?? "";
            var subCategoryName = product.SubCategory?.Name ?? "";
            
            // Handle plural/singular mismatches (e.g. "Vehicles" matches "VehicleProduct")
            var normalizedFilter = _typeFilter.TrimEnd('s');
            
            return typeName.Contains(normalizedFilter, StringComparison.OrdinalIgnoreCase)
                || categoryName.Contains(normalizedFilter, StringComparison.OrdinalIgnoreCase)
                || subCategoryName.Contains(normalizedFilter, StringComparison.OrdinalIgnoreCase);
        }
    }
}

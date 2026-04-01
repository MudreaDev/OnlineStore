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

        public ProductTypeIterator(List<Product> products, string? typeFilter)
        {
            _products = products;
            _typeFilter = typeFilter;
        }

        public bool HasNext()
        {
            // Caută de la poziția curentă înainte
            for (int i = _position; i < _products.Count; i++)
            {
                if (Matches(i)) return true;
            }
            return false;
        }

        public Product Next()
        {
            // Returnează elementul curent și avansează
            while (_position < _products.Count)
            {
                if (Matches(_position))
                {
                    var product = _products[_position];
                    _position++;
                    return product;
                }
                _position++;
            }
            return null!;
        }

        public void Reset() => _position = 0;

        private bool Matches(int index)
        {
            if (index >= _products.Count) return false;
            return string.IsNullOrEmpty(_typeFilter)
                || _typeFilter == "All"
                || _products[index].GetType().Name.StartsWith(_typeFilter);
        }
    }
}

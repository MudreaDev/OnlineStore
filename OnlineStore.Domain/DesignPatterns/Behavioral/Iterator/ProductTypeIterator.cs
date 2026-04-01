using System.Collections.Generic;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Iterator
{
    public class ProductTypeIterator : IProductIterator
    {
        private readonly List<Product> _products;
        private readonly string? _typeFilter;
        private int _position = -1;

        public ProductTypeIterator(List<Product> products, string? typeFilter)
        {
            _products = products;
            _typeFilter = typeFilter;
        }

        public bool HasNext()
        {
            int tempPosition = _position + 1;
            while (tempPosition < _products.Count)
            {
                if (string.IsNullOrEmpty(_typeFilter) || _typeFilter == "All")
                {
                    return true;
                }

                if (_products[tempPosition].GetType().Name.StartsWith(_typeFilter))
                {
                    return true;
                }
                tempPosition++;
            }
            return false;
        }

        public Product Next()
        {
            _position++;
            while (_position < _products.Count)
            {
                if (string.IsNullOrEmpty(_typeFilter) || _typeFilter == "All")
                {
                    return _products[_position];
                }

                if (_products[_position].GetType().Name.StartsWith(_typeFilter))
                {
                    return _products[_position];
                }
                _position++;
            }
            return null!;
        }

        public void Reset()
        {
            _position = -1;
        }
    }
}

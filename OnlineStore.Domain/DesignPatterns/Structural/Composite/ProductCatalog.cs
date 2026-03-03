using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Domain.DesignPatterns.Structural.Composite
{
    // 1. Component Interface
    public interface ICatalogComponent
    {
        string Name { get; }
        decimal GetPrice();
        void Display(int depth);
    }

    // 2. Leaf (Individual Item)
    public class ProductItem : ICatalogComponent
    {
        public string Name { get; }
        public Guid ProductId { get; }
        public string ImageUrl { get; }
        private readonly decimal _price;

        public ProductItem(string name, decimal price, Guid productId = default, string imageUrl = "")
        {
            Name = name;
            _price = price;
            ProductId = productId;
            ImageUrl = imageUrl;
        }

        public decimal GetPrice()
        {
            return _price;
        }

        public void Display(int depth)
        {
            Console.WriteLine(new String('-', depth) + " " + Name + " (" + GetPrice() + " lei)");
        }
    }

    // 3. Composite (Collection of items/other composites)
    public class ProductCategory : ICatalogComponent
    {
        private List<ICatalogComponent> _children = new List<ICatalogComponent>();
        public IReadOnlyList<ICatalogComponent> Children => _children.AsReadOnly();
        public string Name { get; }

        public ProductCategory(string name)
        {
            Name = name;
        }

        public void Add(ICatalogComponent component)
        {
            _children.Add(component);
        }

        public void Remove(ICatalogComponent component)
        {
            _children.Remove(component);
        }

        public decimal GetPrice()
        {
            // Suma prețurilor tuturor elementelor copil
            return _children.Sum(child => child.GetPrice());
        }

        public void Display(int depth)
        {
            Console.WriteLine(new String('-', depth) + " Categorie: " + Name);
            foreach (var child in _children)
            {
                child.Display(depth + 2);
            }
        }
    }
}

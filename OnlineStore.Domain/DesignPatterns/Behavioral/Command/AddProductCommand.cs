using System;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Command
{
    public class AddProductCommand : ICartCommand
    {
        private readonly ShoppingCart _cart;
        private readonly Guid _productId;
        private readonly int _quantity;
        private readonly string? _size;
        private readonly string? _color;

        public AddProductCommand(ShoppingCart cart, Guid productId, string? size = null, string? color = null)
        {
            _cart = cart;
            _productId = productId;
            _size = size;
            _color = color;
            _quantity = 1;
        }

        public AddProductCommand(ShoppingCart cart, Guid productId, int quantity, string? size = null, string? color = null)
        {
            _cart = cart;
            _productId = productId;
            _quantity = quantity;
            _size = size;
            _color = color;
        }

        public void Execute()
        {
            _cart.AddProduct(_productId, _size, _color, _quantity);
        }
    }
}

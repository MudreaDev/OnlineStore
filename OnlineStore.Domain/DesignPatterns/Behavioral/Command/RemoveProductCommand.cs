using System;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Command
{
    public class RemoveProductCommand : ICartCommand
    {
        private readonly ShoppingCart _cart;
        private readonly Guid _productId;
        private readonly string? _size;
        private readonly string? _color;

        public RemoveProductCommand(ShoppingCart cart, Guid productId, string? size = null, string? color = null)
        {
            _cart = cart;
            _productId = productId;
            _size = size;
            _color = color;
        }

        public void Execute()
        {
            _cart.RemoveProduct(_productId, _size, _color);
        }
    }
}

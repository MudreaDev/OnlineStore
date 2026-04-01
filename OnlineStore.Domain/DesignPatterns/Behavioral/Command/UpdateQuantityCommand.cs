using System;
using System.Linq;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.DesignPatterns.Behavioral.Command
{
    public class UpdateQuantityCommand : ICartCommand
    {
        private readonly ShoppingCart _cart;
        private readonly Guid _productId;
        private readonly int _quantity;
        private readonly string? _size;
        private readonly string? _color;

        public UpdateQuantityCommand(ShoppingCart cart, Guid productId, int quantity, string? size = null, string? color = null)
        {
            _cart = cart;
            _productId = productId;
            _quantity = quantity;
            _size = size;
            _color = color;
        }

        public void Execute()
        {
            var item = _cart.Items.FirstOrDefault(i => i.ProductId == _productId && i.Size == _size && i.Color == _color);
            if (item != null)
            {
                if (_quantity > 0)
                {
                    item.Quantity = _quantity;
                }
                else
                {
                    _cart.Items.Remove(item);
                }
            }
        }
    }
}

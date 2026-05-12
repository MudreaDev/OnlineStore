using OnlineStore.Domain.Entities;
using System.Collections.Generic;

namespace OnlineStore.Application.Patterns.Mediator
{
    public interface ICheckoutMediator
    {
        Task<(bool Success, string Message, Order? Order)> CheckoutAsync(
            User user, 
            ShoppingCart cart, 
            string paymentMethod, 
            string storeType, 
            string deliveryType, 
            string shippingProvider, 
            string shippingAddress, 
            string phoneNumber);

        void Notify(object sender, string eventCode);
    }
}

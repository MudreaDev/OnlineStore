using OnlineStore.Application.Patterns.ChainOfResponsibility;
using OnlineStore.Domain.Entities;
using System.Collections.Generic;

namespace OnlineStore.Application.Services
{
    public class OrderValidationService
    {
        private readonly IOrderHandler _handlerChain;

        public OrderValidationService()
        {
            // Setup Chain of Responsibility
            var userValidator = new UserValidationHandler();
            var stockValidator = new StockValidationHandler();
            var pricingValidator = new PricingValidationHandler();

            userValidator.SetNext(stockValidator).SetNext(pricingValidator);
            
            _handlerChain = userValidator;
        }

        public string? ValidateOrder(User user, List<OrderItem> items)
        {
            return _handlerChain.Handle(user, items);
        }
    }
}

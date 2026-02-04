using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Domain.Strategies
{
    // Strategie concretă: aplică discount procentual
    public class PercentageDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _percentage;

        public PercentageDiscountStrategy(decimal percentage)
        {
            _percentage = percentage;
        }

        public decimal ApplyDiscount(decimal price)
        {
            return price - (price * _percentage / 100);
        }
    }
}

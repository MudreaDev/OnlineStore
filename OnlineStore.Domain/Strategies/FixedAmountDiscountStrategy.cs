using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Domain.Strategies
{
    public class FixedAmountDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _discountAmount;

        public FixedAmountDiscountStrategy(decimal discountAmount)
        {
            _discountAmount = discountAmount;
        }

        public decimal ApplyDiscount(decimal total)
        {
            return total - _discountAmount > 0 ? total - _discountAmount : 0;
        }
    }
}

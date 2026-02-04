using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Domain.Strategies
{
	// Strategie concretă: nu aplică niciun discount
	public class NoDiscountStrategy : IDiscountStrategy
	{
		public decimal ApplyDiscount(decimal price)
		{
			return price;
		}
	}
}

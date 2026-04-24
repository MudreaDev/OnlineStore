using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Services
{
    public interface IPaymentService
    {
        void Process(decimal amount);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentService(IPaymentProcessor paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        public void Process(decimal amount)
        {
            _paymentProcessor.ProcessPayment(amount);
        }
    }
}

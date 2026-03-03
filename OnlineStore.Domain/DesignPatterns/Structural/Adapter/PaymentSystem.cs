using System;

namespace OnlineStore.Domain.DesignPatterns.Structural.Adapter
{
    // 1. Target Interface: The standard interface our system expects.
    public interface IExternalPaymentProcessor
    {
        bool ProcessPayment(decimal amount, string currency, string orderId);
    }

    // 2. Adaptee 1: External PayPal API (Incompatible interface)
    public class PayPalApi
    {
        public bool MakePayment(double totalAmount, string currencyCode, string transactionId)
        {
            Console.WriteLine($"[PayPal] Procesare plată: {totalAmount} {currencyCode} pentru Tranzacția {transactionId}");
            return true; // Simplified for demo purposes
        }
    }

    // 3. Adapter 1: PayPal Adapter
    public class PayPalAdapter : IExternalPaymentProcessor
    {
        private readonly PayPalApi _payPalApi;

        public PayPalAdapter(PayPalApi payPalApi)
        {
            _payPalApi = payPalApi;
        }

        public bool ProcessPayment(decimal amount, string currency, string orderId)
        {
            // Translate the call to the PayPal specific interface
            return _payPalApi.MakePayment((double)amount, currency, orderId);
        }
    }

    // 4. Adaptee 2: External Stripe API (Incompatible interface)
    public class StripeApi
    {
        public bool Charge(int amountInCents, string currency, string description)
        {
            Console.WriteLine($"[Stripe] Preluare plată: {amountInCents} cenți {currency} ({description})");
            return true; // Simplified for demo purposes
        }
    }

    // 5. Adapter 2: Stripe Adapter
    public class StripeAdapter : IExternalPaymentProcessor
    {
        private readonly StripeApi _stripeApi;

        public StripeAdapter(StripeApi stripeApi)
        {
            _stripeApi = stripeApi;
        }

        public bool ProcessPayment(decimal amount, string currency, string orderId)
        {
            // Translate the call. Stripe uses cents for amounts.
            int amountInCents = (int)(amount * 100);
            return _stripeApi.Charge(amountInCents, currency, $"Order ID: {orderId}");
        }
    }
}

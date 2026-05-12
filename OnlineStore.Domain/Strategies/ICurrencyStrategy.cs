using System;

namespace OnlineStore.Domain.Strategies
{
    /// <summary>
    /// Interfața pentru Pattern-ul Strategy (Valută).
    /// Permite schimbarea algoritmului de conversie preț în funcție de moneda aleasă.
    /// </summary>
    public interface ICurrencyStrategy
    {
        string CurrencyCode { get; }
        string Symbol { get; }
        decimal Rate { get; set; }
        decimal Convert(decimal amountInBaseCurrency);
    }

    public class MdlCurrencyStrategy : ICurrencyStrategy
    {
        public string CurrencyCode => "MDL";
        public string Symbol => "L";
        public decimal Rate { get; set; } = 18.5m; // Fallback
        public decimal Convert(decimal amount) => amount * Rate;
    }

    public class EurCurrencyStrategy : ICurrencyStrategy
    {
        public string CurrencyCode => "EUR";
        public string Symbol => "€";
        public decimal Rate { get; set; } = 0.92m; // Fallback
        public decimal Convert(decimal amount) => amount * Rate;
    }

    public class UsdCurrencyStrategy : ICurrencyStrategy
    {
        public string CurrencyCode => "USD";
        public string Symbol => "$";
        public decimal Rate { get; set; } = 1.0m;
        public decimal Convert(decimal amount) => amount;
    }
}

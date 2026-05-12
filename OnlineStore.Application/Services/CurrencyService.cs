using OnlineStore.Domain.Strategies;
using Microsoft.AspNetCore.Http;
using System;

namespace OnlineStore.Application.Services
{
    public class CurrencyService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrencyRateProvider _rateProvider;
        private const string CurrencySessionKey = "SelectedCurrency";

        public CurrencyService(IHttpContextAccessor httpContextAccessor, CurrencyRateProvider rateProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _rateProvider = rateProvider;
        }

        public ICurrencyStrategy GetCurrentStrategy()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var currency = session?.GetString(CurrencySessionKey) ?? "USD";

            ICurrencyStrategy strategy = currency switch
            {
                "MDL" => new MdlCurrencyStrategy(),
                "EUR" => new EurCurrencyStrategy(),
                _ => new UsdCurrencyStrategy()
            };

            // Preluăm rata reală de la API
            strategy.Rate = _rateProvider.GetRateAsync(strategy.CurrencyCode).GetAwaiter().GetResult();

            return strategy;
        }

        public void SetCurrency(string currencyCode)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.SetString(CurrencySessionKey, currencyCode);
        }

        public string FormatPrice(decimal baseAmount)
        {
            var strategy = GetCurrentStrategy();
            var converted = strategy.Convert(baseAmount);
            return $"{strategy.Symbol}{converted:N2}";
        }
    }
}

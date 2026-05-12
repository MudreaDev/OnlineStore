using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineStore.Application.Services
{
    /// <summary>
    /// Serviciu care preia ratele de schimb reale dintr-un API extern.
    /// Folosește caching local pentru a evita apelurile excesive.
    /// </summary>
    public class CurrencyRateProvider
    {
        private readonly HttpClient _httpClient;
        private Dictionary<string, decimal> _cachedRates = new Dictionary<string, decimal>();
        private DateTime _lastUpdate = DateTime.MinValue;
        private const string ApiUrl = "https://open.er-api.com/v6/latest/USD";

        public CurrencyRateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetRateAsync(string currencyCode)
        {
            // Cache valid pentru o oră
            if (DateTime.Now - _lastUpdate > TimeSpan.FromHours(1))
            {
                await RefreshRatesAsync();
            }

            if (_cachedRates.ContainsKey(currencyCode))
            {
                return _cachedRates[currencyCode];
            }

            // Fallback la rate fixe dacă API-ul e indisponibil
            return currencyCode switch
            {
                "MDL" => 18.20m,
                "EUR" => 0.92m,
                "USD" => 1.0m,
                _ => 1.0m
            };
        }

        private async Task RefreshRatesAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(ApiUrl);
                var data = JsonDocument.Parse(response);
                var rates = data.RootElement.GetProperty("rates");

                _cachedRates.Clear();
                foreach (var prop in rates.EnumerateObject())
                {
                    _cachedRates[prop.Name] = prop.Value.GetDecimal();
                }

                _lastUpdate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("[CURRENCY] Ratele au fost actualizate de la API.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CURRENCY ERROR] Nu s-a putut apela API-ul: {ex.Message}");
            }
        }
    }
}

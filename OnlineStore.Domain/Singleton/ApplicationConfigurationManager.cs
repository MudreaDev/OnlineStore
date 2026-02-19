using System;

namespace OnlineStore.Domain.Singleton
{
    /// <summary>
    /// Pattern Singleton pentru gestionarea configurației aplicației.
    /// Scop: Garantează că există o singură instanță a setărilor în întreaga aplicație.
    /// Implementat folosind Lazy<T> pentru a asigura thread-safety și inițializare întârziată.
    /// </summary>
    public sealed class ApplicationConfigurationManager
    {
        private static readonly Lazy<ApplicationConfigurationManager> _instance =
            new Lazy<ApplicationConfigurationManager>(() => new ApplicationConfigurationManager());

        public static ApplicationConfigurationManager Instance => _instance.Value;

        public string StoreName { get; private set; } = "Minimal Store";
        public string CurrencySymbol { get; private set; } = "€";
        public decimal VatPercentage { get; private set; } = 19;
        public decimal FreeShippingThreshold { get; private set; } = 500;
        public int MaxItemsPerOrder { get; private set; } = 50;
        public DateTime LastUpdated { get; private set; }

        // Constructor privat pentru a preveni instanțierea externă (esențial pentru Singleton)
        private ApplicationConfigurationManager()
        {
            LastUpdated = DateTime.Now;
        }

        public void UpdateSettings(string storeName, decimal vatPercentage, decimal freeShippingThreshold)
        {
            StoreName = storeName;
            VatPercentage = vatPercentage;
            FreeShippingThreshold = freeShippingThreshold;
            LastUpdated = DateTime.Now;
        }

        public string GetFormattedPrice(decimal price)
        {
            return $"{CurrencySymbol}{price:N2}";
        }
    }
}

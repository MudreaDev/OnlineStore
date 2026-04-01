using System.Collections.Generic;

namespace OnlineStore.Domain.DesignPatterns.Structural.Flyweight
{
    /// <summary>
    /// Flyweight Factory — cache static de ProductTypeFlyweight.
    /// Indiferent de câte produse există, se creează doar câte o instanță per tip.
    /// </summary>
    public class ProductTypeFlyweightFactory
    {
        private static readonly Dictionary<string, ProductTypeFlyweight> _cache = new();
        private static int _requestCount = 0;
        private static int _createdCount = 0;

        public static ProductTypeFlyweight GetFlyweight(string typeName)
        {
            _requestCount++;
            if (!_cache.ContainsKey(typeName))
            {
                _createdCount++;
                _cache[typeName] = typeName switch
                {
                    "Electronic" => new ProductTypeFlyweight(
                        "Electronic", "#ecfdf5", "🔌", "Tech & Gadgets"),
                    "Clothing" => new ProductTypeFlyweight(
                        "Clothing", "#fdf4ff", "👕", "Fashion & Apparel"),
                    "Vehicle" => new ProductTypeFlyweight(
                        "Vehicle", "#fff7ed", "🚗", "Transport & Mobility"),
                    _ => new ProductTypeFlyweight(
                        typeName, "#f3f4f6", "📦", "General Product")
                };
            }
            return _cache[typeName];
        }

        public static int TotalRequests => _requestCount;
        public static int TotalCreated => _createdCount;
        public static int CacheSize => _cache.Count;

        /// <summary>
        /// Resetează cache-ul (util pentru teste).
        /// </summary>
        public static void Reset()
        {
            _cache.Clear();
            _requestCount = 0;
            _createdCount = 0;
        }
    }
}

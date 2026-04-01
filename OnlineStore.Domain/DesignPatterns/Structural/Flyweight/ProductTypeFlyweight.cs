namespace OnlineStore.Domain.DesignPatterns.Structural.Flyweight
{
    /// <summary>
    /// Flyweight — stochează starea intrinsecă (shared) a unui tip de produs.
    /// BadgeColor, Icon, Description sunt identice pentru toate produsele de același tip.
    /// </summary>
    public class ProductTypeFlyweight
    {
        public string TypeName { get; }
        public string BadgeColor { get; }
        public string Icon { get; }
        public string Description { get; }

        public ProductTypeFlyweight(string typeName, string badgeColor, string icon, string description)
        {
            TypeName = typeName;
            BadgeColor = badgeColor;
            Icon = icon;
            Description = description;
        }

        /// <summary>
        /// Combină starea intrinsecă (tipul) cu starea extrinsecă (productName).
        /// </summary>
        public string RenderBadge(string productName)
            => $"[{Icon}] {TypeName} — {productName} ({BadgeColor})";
    }
}

namespace OnlineStore.Domain.DesignPatterns.Structural.Decorator
{
    // The Concrete Component provides default implementation of operations
    public class EmailNotification : INotification
    {
        public string Send(string message)
        {
            return $"[Email] Trimis: {message}";
        }
    }
}

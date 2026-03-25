namespace OnlineStore.Domain.DesignPatterns.Structural.Decorator
{
    // The base Decorator class follows the same interface as other components
    public abstract class NotificationDecorator : INotification
    {
        protected readonly INotification _notification;

        protected NotificationDecorator(INotification notification)
        {
            _notification = notification;
        }

        public virtual string Send(string message)
        {
            return _notification.Send(message);
        }
    }
}

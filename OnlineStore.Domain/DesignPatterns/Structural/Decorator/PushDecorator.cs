namespace OnlineStore.Domain.DesignPatterns.Structural.Decorator
{
    public class PushDecorator : NotificationDecorator
    {
        public PushDecorator(INotification notification) : base(notification) { }

        public override string Send(string message)
        {
            return base.Send(message) + $" + [Push] Notificat: {message}";
        }
    }
}

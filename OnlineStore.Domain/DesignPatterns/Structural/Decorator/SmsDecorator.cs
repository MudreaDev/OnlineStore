namespace OnlineStore.Domain.DesignPatterns.Structural.Decorator
{
    // Concrete Decorators call the wrapped object and alter its result
    public class SmsDecorator : NotificationDecorator
    {
        public SmsDecorator(INotification notification) : base(notification) { }

        public override string Send(string message)
        {
            return base.Send(message) + $" + [SMS] Preluat: {message}";
        }
    }
}

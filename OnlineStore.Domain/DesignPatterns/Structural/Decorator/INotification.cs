namespace OnlineStore.Domain.DesignPatterns.Structural.Decorator
{
    // The Component interface defines operations that can be altered by decorators
    public interface INotification
    {
        string Send(string message);
    }
}

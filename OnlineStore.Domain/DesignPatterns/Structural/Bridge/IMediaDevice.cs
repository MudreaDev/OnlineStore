namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // The Implementor interface defines the implementation-specific operations
    public interface IMediaDevice
    {
        string PlayFile(string fileName, string fileType);
    }
}

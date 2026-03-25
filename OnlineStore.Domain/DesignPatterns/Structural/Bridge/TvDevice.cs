namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // Concrete Implementor: TV
    public class TvDevice : IMediaDevice
    {
        public string PlayFile(string fileName, string fileType)
        {
            return $"[TV] Redare {fileType}: {fileName}";
        }
    }
}

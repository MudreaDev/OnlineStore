namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // Concrete Implementor: Tablet
    public class TabletDevice : IMediaDevice
    {
        public string PlayFile(string fileName, string fileType)
        {
            return $"[Tabletă] Redare {fileType}: {fileName}";
        }
    }
}

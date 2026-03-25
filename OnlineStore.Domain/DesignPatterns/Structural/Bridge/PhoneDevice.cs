namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // Concrete Implementor: Phone
    public class PhoneDevice : IMediaDevice
    {
        public string PlayFile(string fileName, string fileType)
        {
            return $"[Telefon] Redare {fileType}: {fileName}";
        }
    }
}

namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // Refined Abstraction: Audio
    public class AudioFile : MediaFile
    {
        public AudioFile(IMediaDevice device, string fileName) : base(device, fileName) { }

        public override string Play()
        {
            return _device.PlayFile(_fileName, "Audio");
        }
    }
}

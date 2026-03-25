namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // Refined Abstraction: Video
    public class VideoFile : MediaFile
    {
        public VideoFile(IMediaDevice device, string fileName) : base(device, fileName) { }

        public override string Play()
        {
            return _device.PlayFile(_fileName, "Video");
        }
    }
}

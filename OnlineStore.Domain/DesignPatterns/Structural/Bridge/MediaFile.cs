namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // The Abstraction defines the high-level control logic
    public abstract class MediaFile
    {
        protected readonly IMediaDevice _device;
        protected readonly string _fileName;

        protected MediaFile(IMediaDevice device, string fileName)
        {
            _device = device;
            _fileName = fileName;
        }

        public abstract string Play();
    }
}

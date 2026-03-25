using Xunit;
using OnlineStore.Domain.DesignPatterns.Structural.Bridge;

namespace OnlineStore.Tests
{
    public class BridgeTests
    {
        [Fact]
        public void Bridge_AudioOnPhone_ShouldWork()
        {
            // Arrange
            IMediaDevice device = new PhoneDevice();
            MediaFile file = new AudioFile(device, "song.mp3");

            // Act
            string result = file.Play();

            // Assert
            Assert.Equal("[Telefon] Redare Audio: song.mp3", result);
        }

        [Fact]
        public void Bridge_VideoOnTv_ShouldWork()
        {
            // Arrange
            IMediaDevice device = new TvDevice();
            MediaFile file = new VideoFile(device, "movie.mp4");

            // Act
            string result = file.Play();

            // Assert
            Assert.Equal("[TV] Redare Video: movie.mp4", result);
        }

        [Fact]
        public void Bridge_AudioOnTablet_ShouldWork()
        {
            // Arrange
            IMediaDevice device = new TabletDevice();
            MediaFile file = new AudioFile(device, "podcast.wav");

            // Act
            string result = file.Play();

            // Assert
            Assert.Equal("[Tabletă] Redare Audio: podcast.wav", result);
        }
    }
}

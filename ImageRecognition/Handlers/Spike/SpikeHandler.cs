using System.Drawing;
using RadiantConnect.ImageRecognition.Internals;

// ReSharper disable MethodSupportsCancellation
#pragma warning disable CA1416
#if !WINDOWS
#pragma warning disable CS1998
#pragma warning disable CS0649
// ReSharper disable FieldCanBeMadeReadOnly
// ReSharper disable UnusedField
#endif
// ReSharper disable 
namespace RadiantConnect.ImageRecognition.Handlers.Spike
{
    public class SpikeHandler
    {
        internal CancellationTokenSource SpikeCancellationSource = new();
        internal CancellationToken SpikeCancellationToken;

        public delegate void SpikeEvent();
#if WINDOWS
        public event SpikeEvent? OnSpikeActive;
        public event SpikeEvent? OnSpikeDeActive;

        internal int FalseCounter;
        internal bool SpikeActive;

#endif
        public SpikeHandler()
        {
#if !WINDOWS
            throw new PlatformNotSupportedException("This class is only supported on Windows.");
#else
            SpikeCancellationToken = SpikeCancellationSource.Token;
#endif
        }

        public async Task StartSpikeDetection(ColorConfig? colorConfig = null)
        {
#if WINDOWS
            SpikeCancellationSource.TryReset();

            while (!SpikeCancellationToken.IsCancellationRequested)
            {
                Bitmap spikeBox = ImageCaptureHandler.GetSpikeBox();
                bool spikePlanted = ActionDetection.SpikePlantedResult(spikeBox, colorConfig);

                switch (spikePlanted)
                {
                    case true when !SpikeActive:
                        FalseCounter = 0;
                        SpikeActive = true;
                        OnSpikeActive?.Invoke();
                        break;
                    case false when SpikeActive:
                    {
                        if (FalseCounter > 20)
                        {
                            SpikeActive = false;
                            OnSpikeDeActive?.Invoke();
                        }
                        else FalseCounter++;

                        break;
                    }
                }

                spikeBox.Dispose();

                await Task.Delay(10);
            }
#else
            throw new PlatformNotSupportedException("This method is only supported on Windows.");
#endif

        }

        public void StopSpikeDetection() { SpikeCancellationSource.Cancel(); }
    }
}

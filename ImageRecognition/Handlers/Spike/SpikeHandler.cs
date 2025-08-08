using System.Drawing;
using RadiantConnect.ImageRecognition.Internals;

namespace RadiantConnect.ImageRecognition.Handlers.Spike
{
    public class SpikeHandler
    {
        internal CancellationTokenSource SpikeCancellationSource = new();
        internal CancellationToken SpikeCancellationToken;

        public delegate void SpikeEvent();
        public event SpikeEvent? OnSpikeActive;
        public event SpikeEvent? OnSpikeDeActive;

        internal int FalseCounter;
        internal bool SpikeActive;

        public SpikeHandler() => SpikeCancellationToken = SpikeCancellationSource.Token;

        public async Task StartSpikeDetection(ColorConfig? colorConfig = null)
        {
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

                await Task.Delay(10, SpikeCancellationToken);
            }
        }

        public void StopSpikeDetection() => SpikeCancellationSource.Cancel();
    }
}

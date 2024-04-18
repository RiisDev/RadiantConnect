using System.Drawing;
// ReSharper disable MethodSupportsCancellation
#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers
{
    public class SpikeHandler
    {
        public delegate void SpikeEvent();
        public event SpikeEvent? OnSpikeActive;
        public event SpikeEvent? OnSpikeDeActive;

        internal CancellationTokenSource SpikeCancellationSource = new();
        internal CancellationToken SpikeCancellationToken;

        internal int FalseCounter;
        internal bool SpikeActive;

        public SpikeHandler()
        {
            SpikeCancellationToken = SpikeCancellationSource.Token;
        }

        public async Task StartSpikeDetection()
        {
            SpikeCancellationSource.TryReset();

            while (!SpikeCancellationToken.IsCancellationRequested)
            {
                Bitmap spikeBox = CaptureHandler.GetSpikeBox();
                bool spikePlanted = CaptureHandler.SpikePlantedResult(spikeBox);

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
                        else
                        {
                            FalseCounter++;
                        }

                        break;
                    }
                }

                spikeBox.Dispose();

                await Task.Delay(10);
            }
        }

        public void StopSpikeDetection() { SpikeCancellationSource.Cancel(); }
    }
}

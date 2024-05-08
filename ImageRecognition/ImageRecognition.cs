using System.Drawing;
using RadiantConnect.ImageRecognition.Handlers.KillFeed;
using RadiantConnect.ImageRecognition.Handlers.Spike;
using RadiantConnect.ImageRecognition.Internals;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CA1416 // idk btimap only supported on windows

namespace RadiantConnect.ImageRecognition
{

    public class ImageRecognition
    {
        public delegate void HandlerCreated<in T>(T value);

        public event HandlerCreated<KillFeedHandler>? OnKillFeedHandlerCreated; 
        public event HandlerCreated<SpikeHandler>? OnSpikeHandlerCreated; 

        public KillFeedHandler? KillFeedHandler { get; internal set; }
        public SpikeHandler? SpikeHandler { get; internal set; }

        internal static void DrawDebugLine(Bitmap bitmap, int x, int y, int width, Color color)
        {
            using Graphics g = Graphics.FromImage(bitmap);
            using Pen pen = new(color, width);
            g.DrawLine(pen, x, y, x + width, y);
        }

        public void Initiator(Config config)
        {
            KillFeedConfig feedConfig = config.KillFeedConfig;

            if (feedConfig.CheckAssists || feedConfig.CheckKilled || feedConfig.CheckWasKilled) Task.Run(() =>
            {
                KillFeedHandler = new KillFeedHandler();
                OnKillFeedHandlerCreated?.Invoke(KillFeedHandler);
                KillFeedHandler.StartKillDetection(feedConfig);
            });

            if (config.SpikePlanted)
            {
                SpikeHandler = new SpikeHandler();
                OnSpikeHandlerCreated?.Invoke(SpikeHandler);
                SpikeHandler.StartSpikeDetection();
            }

        }
    }
}

using System.Drawing;

// ReSharper disable MethodSupportsCancellation
#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers
{
    public class KillFeedHandler
    {
        public delegate void KillFeedEvent();
        public event KillFeedEvent? OnKill;
        public event KillFeedEvent? OnAssist;
        public event KillFeedEvent? OnDeath;

        internal CancellationTokenSource KillFeedCancellationSource = new();
        internal CancellationToken KillFeedCancellationToken;
        internal List<KillFeedPositions> KillCache = [];

        public KillFeedHandler()
        {
            KillFeedCancellationToken = KillFeedCancellationSource.Token;
            Task.Run( async () =>
            {
                while (true)
                {
                    await Task.Delay(6000);
                    if (KillCache.Count == 0) continue;
                    KillCache.RemoveAt(0);
                }
            });
        }

        public async Task StartKillDetection(KillFeedConfig config)
        {
            KillFeedCancellationSource.TryReset();
            
            while (!KillFeedCancellationToken.IsCancellationRequested)
            {

                const int killBoxOffset = 94;

                List<Bitmap> killBoxes = [
                    CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset), new Size(0, 38)),
                    CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 39), new Size(0, 38)),
                    CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 78), new Size(0, 38)),
                    CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 160), new Size(0, 38)),
                    CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 316), new Size(0, 38)),
                    CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 628), new Size(0, 38)),
                ];
                
                foreach (Bitmap killBox in killBoxes)
                {
                    KillFeedPositions killPositions = PositionHandler.GetKillHalfPosition(killBox);
                    if (killPositions.GreenPixel <= 5 || killPositions.Middle <= 5 || killPositions.RedPixel <= 5) continue; // Check if position is even valid

                    KillFeedAction actionResult = CaptureHandler.ActionResult(killBox, killPositions);

                    bool cacheFound = false;

                    foreach (KillFeedPositions cachedPosition in KillCache)
                    {
                        cachedPosition.Deconstruct(out int cacheRed, out int cacheGreen, out int cacheMiddle);
                        killPositions.Deconstruct(out int red, out int green, out int middle);
                        if (cacheRed == red && cacheGreen == green && cacheMiddle == middle) cacheFound = true;
                    }

                    if (cacheFound) continue;

                    if (config.CheckAssists && actionResult.WasAssist)
                        OnAssist?.Invoke();
                    if (config.CheckKilled && actionResult.PerformedKill)
                        OnKill?.Invoke();
                    if (config.CheckWasKilled && actionResult.WasKilled)
                        OnDeath?.Invoke();

                    KillCache.Add(killPositions);

                    killBox.Dispose();
                }

                await Task.Delay(250);
            }
        }

        public void StopKillDetection() { KillFeedCancellationSource.Cancel(); }
    }
}

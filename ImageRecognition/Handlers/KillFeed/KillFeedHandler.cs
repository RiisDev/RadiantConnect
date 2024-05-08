using System.Drawing;
using RadiantConnect.ImageRecognition.Internals;

// ReSharper disable MethodSupportsCancellation
#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers.KillFeed
{
    public class KillFeedHandler
    {
        public delegate void KillFeedEvent();
        public event KillFeedEvent? OnKill;
        public event KillFeedEvent? OnAssist;
        public event KillFeedEvent? OnDeath;

        internal CancellationTokenSource KillFeedCancellationSource = new();
        internal CancellationToken KillFeedCancellationToken;

        internal Dictionary<TimeOnly, int> TimestampIndex = [];

        internal Dictionary<int, string> LastCalled = new()
        {
            { 0, "" },
            { 1, "" },
            { 2, "" },
            { 3, "" },
            { 4, "" },
            { 5, "" },
        };

        internal Dictionary<int, int> RedPixelLog = new()
        {
            { 0, 0 },
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 },
        };

        public KillFeedHandler()
        {
            KillFeedCancellationToken = KillFeedCancellationSource.Token;
        }

        public async Task StartKillDetection(KillFeedConfig config)
        {
            KillFeedCancellationSource.TryReset();

            while (!KillFeedCancellationToken.IsCancellationRequested)
            {

                const int killBoxOffset = 94;
                Dictionary<Bitmap, KillFeedAction> killBoxes = [];

                Dictionary<Bitmap, KillFeedPositions?> killBoxesTemp = new()
                {
                    { ImageCaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset), new Size(0, 38)), null },
                    //{ ImageCaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 39), new Size(0, 38)), null },
                    //{ ImageCaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 78), new Size(0, 38)), null },
                    //{ ImageCaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 160), new Size(0, 38)), null },
                    //{ ImageCaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 316), new Size(0, 38)), null },
                    //{ ImageCaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 628), new Size(0, 38)), null },
                };

                foreach (Bitmap killBoxBitmap in killBoxesTemp.Keys)
                {
                    KillFeedPositions killPositions = ActionDetection.GetKillHalfPosition(killBoxBitmap);
                    if (!killPositions.ValidPosition) continue;

                    KillFeedAction actionResult = ActionDetection.ActionResult(killBoxBitmap, killPositions);
                    if (!actionResult.WasInFeed) continue;

                    killBoxes.Add(killBoxBitmap, actionResult);
                }

                killBoxesTemp.Clear();

                foreach ((Bitmap killBox, KillFeedAction actionResult) in killBoxes)
                {
                    await Task.Run(() =>
                    {
                        KillFeedPositions killPositions = actionResult.Positions;
                        TimeOnly killTime = killPositions.KillTime;

                        int killBoxIndex = killBoxes.Keys.ToList().IndexOf(killBox);
                        string lastCalled = actionResult.PerformedKill ? "Kill" : actionResult.WasKilled ? "Death" : actionResult.WasAssist ? "Assist" : "Unknown";

                        bool timestampDetection = TimestampIndex.Any(data => data.Value == killBoxIndex && LastCalled[killBoxIndex] == lastCalled && data.Key.IsWithinFourSeconds(killTime));
                        bool canAdd = TimestampIndex.TryAdd(killTime, killBoxIndex);

                        if (timestampDetection) return;
                        if (!canAdd) return;

                        LastCalled[killBoxIndex] = lastCalled;
                        RedPixelLog[killBoxIndex] = killPositions.RedPixel;

                        if (killBoxIndex < 6 && killPositions.RedPixel.IsClose(RedPixelLog[killBoxIndex + 1], 4)) return;

                        if (config.CheckKilled && actionResult.PerformedKill)
                            OnKill?.Invoke();
                        else if (config.CheckWasKilled && actionResult.WasKilled)
                            OnDeath?.Invoke();
                        else if (config.CheckAssists && actionResult.WasAssist)
                            OnAssist?.Invoke();

                        killBox.Dispose();
                    });
                }

                await Task.Delay(1);
            }
        }

        public void StopKillDetection() { KillFeedCancellationSource.Cancel(); }
    }
}

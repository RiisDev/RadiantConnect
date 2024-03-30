using System.Diagnostics;
using System.Drawing;

// ReSharper disable MethodSupportsCancellation
#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers
{
    public class KillFeedHandler
    {
        public delegate void KillFeedEvent(string data);
        public event KillFeedEvent? OnKill;
        public event KillFeedEvent? OnAssist;
        public event KillFeedEvent? OnDeath;

        internal CancellationTokenSource KillFeedCancellationSource = new();
        internal CancellationToken KillFeedCancellationToken;

        internal Dictionary<TimeOnly,int> TimestampIndex = [];

        internal Dictionary<int, string> LastCalled = new()
        {
            { 0, "" },
            { 1, "" },
            { 2, "" },
            { 3, "" },
            { 4, "" },
            { 5, "" },
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
                    { CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset), new Size(0, 38)), null },
                    { CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 39), new Size(0, 38)), null },
                    { CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 78), new Size(0, 38)), null },
                    { CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 160), new Size(0, 38)), null },
                    { CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 316), new Size(0, 38)), null },
                    { CaptureHandler.GetKillFeedBox(new Point(0, killBoxOffset + 628), new Size(0, 38)), null },
                };

                foreach (Bitmap killBoxBitmap in killBoxesTemp.Keys)
                {
                    KillFeedPositions killPositions = PositionHandler.GetKillHalfPosition(killBoxBitmap);
                    if (!killPositions.ValidPosition) continue;

                    KillFeedAction actionResult = CaptureHandler.ActionResult(killBoxBitmap, killPositions);
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

                        if (config.CheckKilled && actionResult.PerformedKill)
                            OnKill?.Invoke(killBoxIndex.ToString());
                        else if (config.CheckWasKilled && actionResult.WasKilled)
                            OnDeath?.Invoke(killBoxIndex.ToString());
                        else if (config.CheckAssists && actionResult.WasAssist)
                            OnAssist?.Invoke(killBoxIndex.ToString());

                        killBox.Dispose();
                    });
                }

                await Task.Delay(10);
            }
        }

        public void StopKillDetection() { KillFeedCancellationSource.Cancel(); }
    }
}

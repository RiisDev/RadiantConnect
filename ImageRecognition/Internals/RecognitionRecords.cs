namespace RadiantConnect.ImageRecognition.Internals;

internal record KillFeedPositions(int RedPixel, int GreenPixel, int Middle, bool ValidPosition, TimeOnly KillTime);

internal record KillFeedAction(bool PerformedKill, bool WasKilled, bool WasAssist, bool WasInFeed, KillFeedPositions Positions);

public record KillFeedConfig(bool CheckKilled, bool CheckAssists, bool CheckWasKilled);

public record Config(KillFeedConfig KillFeedConfig, bool SpikePlanted);
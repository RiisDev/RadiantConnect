using System.Drawing;

namespace RadiantConnect.ImageRecognition.Internals;

public record SpikeColorConfig(Color LowestColor, Color HighestColor);

public record ActionColorConfig(Color LowestColor, Color HighestColor);

public record RedConfig(Color LowestColor, Color HighestColor);

public record GreenConfig(Color LowestColor, Color HighestColor);

public record ColorConfig(SpikeColorConfig SpikeColorConfig, ActionColorConfig ActionColorConfig, RedConfig RedConfig, GreenConfig GreenConfig);

internal record KillFeedPositions(int RedPixel, int GreenPixel, int Middle, bool ValidPosition, TimeOnly KillTime);

internal record KillFeedAction(bool PerformedKill, bool WasKilled, bool WasAssist, bool WasInFeed, KillFeedPositions Positions);

public record KillFeedConfig(bool CheckKilled, bool CheckAssists, bool CheckWasKilled);

public record Config(KillFeedConfig KillFeedConfig, bool SpikePlanted, ColorConfig? ColorConfig = null);
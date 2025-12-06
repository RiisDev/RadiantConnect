using RadiantConnect.Network.PVPEndpoints.DataTypes;

namespace RadiantConnect.RConnect.DataTypes
{
	/// <summary>
	/// Represents aggregated performance statistics for a player in a match.
	/// </summary>
	/// <param name="RoundsPlayed">The total number of rounds the player participated in.</param>
	/// <param name="PlayerKills">The total number of kills earned by the player.</param>
	/// <param name="PlayerAssists">The number of assists recorded by the player.</param>
	/// <param name="PlayerDeaths">The total number of times the player died.</param>
	/// <param name="AverageCombatScore">The player's average combat score for the match.</param>
	/// <param name="Plants">The number of spike plants performed by the player.</param>
	/// <param name="Defuses">The number of spike defuses performed by the player.</param>
	/// <param name="EconomyRating">The player’s overall economic rating in the match.</param>
	/// <param name="FirstBloods">The number of rounds in which the player secured first blood.</param>
	public record Stats(
		long RoundsPlayed,
		long PlayerKills,
		long PlayerAssists,
		long PlayerDeaths,
		long AverageCombatScore,
		long Plants,
		long Defuses,
		long EconomyRating,
		long FirstBloods
	);
	
	/// <summary>
	/// Represents a player participating in a match along with match-specific performance data.
	/// </summary>
	/// <param name="Puuid">The unique identifier of the player.</param>
	/// <param name="GameName">The player's in-game display name.</param>
	/// <param name="TagLine">The player's tagline or discriminator.</param>
	/// <param name="TeamId">The identifier of the team the player belongs to.</param>
	/// <param name="Character">The agent or character selected by the player.</param>
	/// <param name="Won">Indicates whether the player was on the winning team.</param>
	/// <param name="AccountLevel">The account level of the player.</param>
	/// <param name="Stats">The player's match-specific performance statistics.</param>
	/// <param name="AbilityCasts">Information regarding the player's ability usage.</param>
	public record Player(
		string Puuid,
		string GameName,
		string TagLine,
		string TeamId,
		string Character,
		bool Won,
		int AccountLevel,
		Stats Stats,
		AbilityCasts AbilityCasts
	);
	
	/// <summary>
	/// Represents detailed metadata and player statistics for a completed match.
	/// </summary>
	/// <param name="MatchId">The unique identifier of the match.</param>
	/// <param name="MapName">The name of the map the match was played on.</param>
	/// <param name="Pod">The server pod or region identifier.</param>
	/// <param name="QueueId">The queue or playlist the match was played in.</param>
	/// <param name="SeasonId">The identifier of the season the match belongs to.</param>
	/// <param name="WinningTeam">The team that won the match.</param>
	/// <param name="Players">A read-only list of players who participated in the match.</param>
	public record MatchStats(
		string MatchId,
		string MapName,
		string Pod,
		string QueueId,
		string SeasonId,
		string WinningTeam,
		IReadOnlyList<Player> Players
	);
}
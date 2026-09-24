namespace RadiantConnect.Network.PartyEndpoints.DataTypes
{
	/// <summary>
	/// Represents the type of solo (non-multiplayer) experience to start for the current party.
	/// </summary>
	public enum SoloExperienceType
	{
		/// <summary>The shooting range practice area.</summary>
		ShootingRange,

		/// <summary>A bot training match.</summary>
		BotTrainingMatch,

		/// <summary>The new player experience tutorial.</summary>
		NewPlayerExperience,

		/// <summary>Watching a match replay.</summary>
		WatchReplay,

		/// <summary>The new player experience replay.</summary>
		ReplayNewPlayerExperience
	}
}

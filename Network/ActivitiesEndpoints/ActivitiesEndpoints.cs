using RadiantConnect.Network.ActivitiesEndpoints.DataTypes;
using RadiantConnect.Services;

namespace RadiantConnect.Network.ActivitiesEndpoints
{
	/// <summary>
	/// Provides access to activity endpoints, such as looking up party activities and managing shared join links.
	/// </summary>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing authentication,
	/// networking, and client context.
	/// </param>
	public class ActivitiesEndpoints(Initiator initiator)
	{
		internal string Url = $"https://{GetSgpCluster(initiator.ExternalSystem.ClientData.Shard)}.pp.sgp.pvp.net";

		// ponytail: SGP cluster isn't exposed on ClientData; mapped from the shard the same way Riot's own affinity routing does.
		private static string GetSgpCluster(LogService.ClientData.ShardType shard) => shard switch
		{
			LogService.ClientData.ShardType.Eu => "euc1",
			LogService.ClientData.ShardType.Ap => "apse1",
			LogService.ClientData.ShardType.Kr => "apne1",
			_ => "usw2" // Na, Latam, Br
		};

		/// <summary>
		/// Looks up the activity ID for a party.
		/// </summary>
		/// <param name="partyId">The party ID to look up.</param>
		/// <returns>
		/// A <see cref="PartyActivity"/> instance containing the activity ID, or <c>null</c> if the request fails.
		/// </returns>
		public async Task<PartyActivity?> FetchActivityAsync(string partyId) =>
			await initiator.ExternalSystem.Net.PostAsync<PartyActivity>(Url, "activities/v1/lookup", JsonContent.Create(new { product = "valorant", party = partyId })).ConfigureAwait(false);

		/// <summary>
		/// Creates a shared join link (join code) for an activity.
		/// </summary>
		/// <param name="activityId">The activity ID, from <see cref="FetchActivityAsync"/>.</param>
		/// <returns>
		/// A <see cref="SharedJoinLink"/> instance, or <c>null</c> if the request fails.
		/// </returns>
		public async Task<SharedJoinLink?> CreateSharedJoinLinkAsync(string activityId) =>
			await initiator.ExternalSystem.Net.PostAsync<SharedJoinLink>(Url, $"activities/v1/{activityId}/join-codes/shared").ConfigureAwait(false);

		/// <summary>
		/// Disables the shared join link for an activity, invalidating its join code.
		/// </summary>
		/// <param name="activityId">The activity ID, from <see cref="FetchActivityAsync"/>.</param>
		public async Task DisableSharedJoinLinkAsync(string activityId) =>
			await initiator.ExternalSystem.Net.DeleteAsync(Url, $"activities/v1/{activityId}/join-codes/shared").ConfigureAwait(false);
	}
}

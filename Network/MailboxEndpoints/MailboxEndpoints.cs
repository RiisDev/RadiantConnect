using RadiantConnect.Network.MailboxEndpoints.DataTypes;
using RadiantConnect.Services;

namespace RadiantConnect.Network.MailboxEndpoints
{
	/// <summary>
	/// Provides access to the player's in-game mailbox, including reading and deleting mail.
	/// </summary>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing authentication,
	/// networking, and client context.
	/// </param>
	public class MailboxEndpoints(Initiator initiator)
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
		/// Fetches the authenticated player's mailbox.
		/// </summary>
		/// <param name="count">The maximum number of mail entries to return.</param>
		/// <param name="startIndex">The pagination start index.</param>
		/// <param name="includedStates">Optional mail states to filter by (NEW, READ, ACKNOWLEDGED).</param>
		/// <returns>
		/// A list of <see cref="Mail"/> instances, or <c>null</c> if the request fails.
		/// </returns>
		public async Task<IReadOnlyList<Mail>?> FetchMailboxAsync(int? count = null, int? startIndex = null, string? includedStates = null) =>
			await initiator.ExternalSystem.Net.GetAsync<IReadOnlyList<Mail>>(Url, $"mailbox/v1/{initiator.Client.UserId}/product/valorant?count={count}&startIndex={startIndex}&includedStates={includedStates}").ConfigureAwait(false);

		/// <summary>
		/// Fetches a specific mail entry by its ID.
		/// </summary>
		/// <param name="mailId">The mail identifier, from <see cref="FetchMailboxAsync"/>.</param>
		/// <returns>
		/// A <see cref="Mail"/> instance, or <c>null</c> if the request fails.
		/// </returns>
		public async Task<Mail?> FetchMailAsync(string mailId) =>
			await initiator.ExternalSystem.Net.GetAsync<Mail>(Url, $"mailbox/v1/{initiator.Client.UserId}/mail/{mailId}").ConfigureAwait(false);

		/// <summary>
		/// Deletes a specific mail entry by its ID.
		/// </summary>
		/// <param name="mailId">The mail identifier, from <see cref="FetchMailboxAsync"/>.</param>
		public async Task DeleteMailAsync(string mailId) =>
			await initiator.ExternalSystem.Net.DeleteAsync(Url, $"mailbox/v1/{initiator.Client.UserId}/mail/{mailId}").ConfigureAwait(false);
	}
}

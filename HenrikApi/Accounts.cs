using System.Text.Json.Serialization;

namespace RadiantConnect.HenrikApi
{
    public class Accounts(HenrikClient henrikClient)
    {
        public async Task<Account?> GetAccountByNameTagAsync(string name, string tag) =>
            await henrikClient.GetAsync<Account?>($"/account/{name}/{tag}");

        public async Task<AccountUuid?> GetAccountByPuuidAsync(string puuid) =>
            await henrikClient.GetAsync<AccountUuid?>($"/by-puuid/account/{puuid}");

        public record AccountData(
            [property: JsonPropertyName("puuid")] string Puuid,
            [property: JsonPropertyName("region")] string Region,
            [property: JsonPropertyName("account_level")] int? AccountLevel,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("tag")] string Tag,
            [property: JsonPropertyName("card")] string Card,
            [property: JsonPropertyName("title")] string Title,
            [property: JsonPropertyName("platforms")] IReadOnlyList<string> Platforms,
            [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt
        );

        public record Card(
            [property: JsonPropertyName("small")] string Small,
            [property: JsonPropertyName("large")] string Large,
            [property: JsonPropertyName("wide")] string Wide,
            [property: JsonPropertyName("id")] string Id
        );

        public record AccountUuidData(
            [property: JsonPropertyName("puuid")] string Puuid,
            [property: JsonPropertyName("region")] string Region,
            [property: JsonPropertyName("account_level")] int? AccountLevel,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("tag")] string Tag,
            [property: JsonPropertyName("card")] Card Card,
            [property: JsonPropertyName("last_update")] string LastUpdate,
            [property: JsonPropertyName("last_update_raw")] int? LastUpdateRaw
        );

        public record Account(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] AccountData Data
        );

        public record AccountUuid(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] AccountUuidData Data
        );
    }  
}

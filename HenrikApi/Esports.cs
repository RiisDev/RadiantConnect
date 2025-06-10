using System.Text.Json.Serialization;
// ReSharper disable InconsistentNaming

namespace RadiantConnect.HenrikApi
{
    public class Esports(HenrikClient henrikClient)
    {
        public enum Region
        {
            eu,
            na,
            ap,
            kr
        }

        public enum LeagueType
        {
            vct_americas,
            challengers_na,
            game_changers_na,
            vct_emea,
            vct_pacific,
            challengers_br,
            challengers_jpn,
            challengers_kr,
            challengers_latam,
            challengers_latam_n,
            challengers_latam_s,
            challengers_apac,
            challengers_sea_id,
            challengers_sea_ph,
            challengers_sea_sg_and_my,
            challengers_sea_th,
            challengers_sea_hk_and_tw,
            challengers_sea_vn,
            valorant_oceania_tour,
            challengers_south_asia,
            game_changers_sea,
            game_changers_series_brazil,
            game_changers_east_asia,
            game_changers_emea,
            game_changers_jpn,
            game_changers_kr,
            game_changers_latam,
            game_changers_championship,
            masters,
            last_chance_qualifier_apac,
            last_chance_qualifier_east_asia,
            last_chance_qualifier_emea,
            last_chance_qualifier_na,
            last_chance_qualifier_br_and_latam,
            vct_lock_in,
            champions,
            vrl_spain,
            vrl_northern_europe,
            vrl_dach,
            vrl_france,
            vrl_east,
            vrl_turkey,
            vrl_cis,
            mena_resilence,
            challengers_italy,
            challengers_portugal
        }

        public async Task<EsportsScheduleData?> GetScheduleAsync(Region region, LeagueType league) =>
            await henrikClient.GetAsync<EsportsScheduleData?>($"/valorant/v1/esports/schedule?region={region}&league={league}");

        public record EsportsScheduleDatum(
            [property: JsonPropertyName("date")] DateTime? Date,
            [property: JsonPropertyName("state")] string State,
            [property: JsonPropertyName("type")] string Type,
            [property: JsonPropertyName("vod")] string Vod,
            [property: JsonPropertyName("league")] League League,
            [property: JsonPropertyName("tournament")] Tournament Tournament,
            [property: JsonPropertyName("match")] Match Match
        );

        public record GameType(
            [property: JsonPropertyName("type")] string Type,
            [property: JsonPropertyName("count")] int? Count
        );

        public record League(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("identifier")] string Identifier,
            [property: JsonPropertyName("icon")] string Icon,
            [property: JsonPropertyName("region")] string Region
        );

        public record Match(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("game_type")] GameType GameType,
            [property: JsonPropertyName("teams")] IReadOnlyList<Team> Teams
        );

        public record Record(
            [property: JsonPropertyName("wins")] int? Wins,
            [property: JsonPropertyName("losses")] int? Losses
        );

        public record EsportsScheduleData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<EsportsScheduleDatum> Data
        );

        public record Team(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("code")] string Code,
            [property: JsonPropertyName("icon")] string Icon,
            [property: JsonPropertyName("has_won")] bool? HasWon,
            [property: JsonPropertyName("game_wins")] int? GameWins,
            [property: JsonPropertyName("record")] Record Record
        );

        public record Tournament(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("season")] string Season
        );
    }
}

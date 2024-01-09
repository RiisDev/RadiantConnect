## _21 Record

The `_21` record represents specific details related to a tier in the RadiantConnect network PVP endpoints.

### Properties

- **RankedRatingThreshold**
  - Type: `long?`
  - Description: Represents the threshold for the ranked rating in the tier.

- **StartingPage**
  - Type: `long?`
  - Description: Represents the starting page for the tier.

- **StartingIndex**
  - Type: `long?`
  - Description: Represents the starting index for the tier.

## _24 Record

The `_24` record represents specific details related to a tier in the RadiantConnect network PVP endpoints.

### Properties

- **RankedRatingThreshold**
  - Type: `long?`
  - Description: Represents the threshold for the ranked rating in the tier.

- **StartingPage**
  - Type: `long?`
  - Description: Represents the starting page for the tier.

- **StartingIndex**
  - Type: `long?`
  - Description: Represents the starting index for the tier.

## LeaderboardPlayer Record

The `LeaderboardPlayer` record represents a player in the leaderboard in the RadiantConnect network PVP endpoints.

### Properties

- **PlayerCardID**
  - Type: `string`
  - Description: Represents the unique identifier for the player card.

- **TitleID**
  - Type: `string`
  - Description: Represents the title identifier for the player.

- **IsBanned**
  - Type: `bool?`
  - Description: Indicates whether the player is banned.

- **IsAnonymized**
  - Type: `bool?`
  - Description: Indicates whether the player is anonymized.

- **Puuid**
  - Type: `string`
  - Description: Represents the PUUID (Portable Unique ID) of the player.

- **GameName**
  - Type: `string`
  - Description: Represents the in-game name of the player.

- **TagLine**
  - Type: `string`
  - Description: Represents the tag line associated with the player.

- **LeaderboardRank**
  - Type: `long?`
  - Description: Represents the rank of the player in the leaderboard.

- **RankedRating**
  - Type: `long?`
  - Description: Represents the ranked rating of the player.

- **NumberOfWins**
  - Type: `long?`
  - Description: Represents the number of wins the player has.

- **CompetitiveTier**
  - Type: `long?`
  - Description: Represents the competitive tier of the player.

## Leaderboard Record

The `Leaderboard` record represents the leaderboard in the RadiantConnect network PVP endpoints.

### Properties

- **Deployment**
  - Type: `string`
  - Description: Represents the deployment associated with the leaderboard.

- **QueueID**
  - Type: `string`
  - Description: Represents the unique identifier for the queue.

- **SeasonID**
  - Type: `string`
  - Description: Represents the unique identifier for the season.

- **Players**
  - Type: `IReadOnlyList<LeaderboardPlayer>`
  - Description: Represents a list of players in the leaderboard.

- **TotalPlayers**
  - Type: `long?`
  - Description: Represents the total number of players in the leaderboard.

- **ImmortalStartingPage**
  - Type: `long?`
  - Description: Represents the starting page for the Immortal tier.

- **ImmortalStartingIndex**
  - Type: `long?`
  - Description: Represents the starting index for the Immortal tier.

- **TopTierRRThreshold**
  - Type: `long?`
  - Description: Represents the threshold for the top tier ranked rating.

- **TierDetails**
  - Type: `TierDetails`
  - Description: Represents details about different tiers in the leaderboard.

- **StartIndex**
  - Type: `long?`
  - Description: Represents the starting index for the leaderboard.

- **Query**
  - Type: `string`
  - Description: Represents the query associated with the leaderboard.

## TierDetails Record

The `TierDetails` record represents details about different tiers in the RadiantConnect network PVP endpoints.

### Properties

- **_21**
  - Type: `_21`
  - Description: Represents details about the tier with identifier "21".

- **_24**
  - Type: `_24`
  - Description: Represents details about the tier with identifier "24".

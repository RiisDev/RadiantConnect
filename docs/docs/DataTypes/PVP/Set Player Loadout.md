## SetPlayerLoadout Record

The `SetPlayerLoadout` record represents a set of player loadout information used for updating player loadout settings in the RadiantConnect network PVP endpoints.

### Properties

- **Guns**
  - Type: `List<Gun>`
  - Description: Represents a list of guns to be set in the player's loadout.

- **Sprays**
  - Type: `List<Spray>`
  - Description: Represents a list of sprays to be set in the player's loadout.

- **Identity**
  - Type: `Identity`
  - Description: Represents the identity information associated with the player.

- **Incognito**
  - Type: `bool?`
  - Description: Indicates whether the player should be in incognito mode.

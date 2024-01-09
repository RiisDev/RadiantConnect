## PlayerLoadout Record

The `PlayerLoadout` record represents the loadout information for a player in the RadiantConnect network PVP endpoints.

### Properties

- **Subject**
  - Type: `string`
  - Description: Represents the subject associated with the player loadout.

- **Version**
  - Type: `long?`
  - Description: Represents the version of the player loadout information.

- **Guns**
  - Type: `IReadOnlyList<Gun>`
  - Description: Represents a list of guns in the player's loadout.

- **Sprays**
  - Type: `IReadOnlyList<Spray>`
  - Description: Represents a list of sprays in the player's loadout.

- **Identity**
  - Type: `Identity`
  - Description: Represents the identity information associated with the player.

- **Incognito**
  - Type: `bool?`
  - Description: Indicates whether the player is in incognito mode.

## Gun Record

The `Gun` record represents information about a gun in the player's loadout.

### Properties

- **ID**
  - Type: `string`
  - Description: Represents the ID of the gun.

- **SkinID**
  - Type: `string`
  - Description: Represents the ID of the skin associated with the gun.

- **SkinLevelID**
  - Type: `string`
  - Description: Represents the ID of the skin level associated with the gun.

- **ChromaID**
  - Type: `string`
  - Description: Represents the ID of the chroma associated with the gun.

- **CharmInstanceID**
  - Type: `string`
  - Description: Represents the ID of the charm instance associated with the gun.

- **CharmID**
  - Type: `string`
  - Description: Represents the ID of the charm associated with the gun.

- **CharmLevelID**
  - Type: `string`
  - Description: Represents the ID of the charm level associated with the gun.

- **Attachments**
  - Type: `IReadOnlyList<object>`
  - Description: Represents a list of attachments associated with the gun. The exact structure of each attachment object may vary.

## Spray Record

The `Spray` record represents information about a spray in the player's loadout.

### Properties

- **EquipSlotID**
  - Type: `string`
  - Description: Represents the ID of the equip slot associated with the spray.

- **SprayID**
  - Type: `string`
  - Description: Represents the ID of the spray.

- **SprayLevelID**
  - Type: `object`
  - Description: Represents the ID of the spray level associated with the spray.

## Identity Record

The `Identity` record represents identity information associated with the player.

### Properties

- **PlayerCardID**
  - Type: `string`
  - Description: Represents the ID of the player card associated with the player.

- **PlayerTitleID**
  - Type: `string`
  - Description: Represents the ID of the player title associated with the player.

- **AccountLevel**
  - Type: `long?`
  - Description: Represents the account level of the player.

- **PreferredLevelBorderID**
  - Type: `string`
  - Description: Represents the ID of the preferred level border associated with the player.

- **HideAccountLevel**
  - Type: `bool?`
  - Description: Indicates whether the account level should be hidden.

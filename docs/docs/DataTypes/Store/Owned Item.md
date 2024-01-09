## Entitlement Record

The `Entitlement` record represents an entitlement within the RadiantConnect network.

### Properties

#### `TypeID`

- Type: `string`
- Description: Represents the unique identifier for the entitlement type.

#### `ItemID`

- Type: `string`
- Description: Represents the unique identifier for the item associated with the entitlement.

## OwnedItem Record

The `OwnedItem` record represents an owned item along with its entitlements within the RadiantConnect network.

### Properties

#### `ItemTypeID`

- Type: `string`
- Description: Represents the unique identifier for the item type.

#### `Entitlements`

- Type: `IReadOnlyList<Entitlement>`
- Description: Represents a list of entitlements associated with the owned item.


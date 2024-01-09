## Event Record

The `Event` record represents an event in the RadiantConnect network related to PVP endpoints.

### Properties

- **ID**
  - Type: `string`
  - Description: Represents the unique identifier for the event.

- **Name**
  - Type: `string`
  - Description: Represents the name of the event.

- **StartTime**
  - Type: `DateTime?`
  - Description: Represents the start time of the event.

- **EndTime**
  - Type: `DateTime?`
  - Description: Represents the end time of the event.

- **IsActive**
  - Type: `bool?`
  - Description: Indicates whether the event is currently active.

## Content Record

The `Content` record represents content related to PVP endpoints in the RadiantConnect network.

### Properties

- **DisabledIDs**
  - Type: `IReadOnlyList<object>`
  - Description: Represents a list of disabled IDs.

- **Seasons**
  - Type: `IReadOnlyList<Season>`
  - Description: Represents a list of seasons associated with the content.

- **Events**
  - Type: `IReadOnlyList<Event>`
  - Description: Represents a list of events associated with the content.

## Season Record

The `Season` record represents a season in the RadiantConnect network related to PVP endpoints.

### Properties

- **ID**
  - Type: `string`
  - Description: Represents the unique identifier for the season.

- **Name**
  - Type: `string`
  - Description: Represents the name of the season.

- **Type**
  - Type: `string`
  - Description: Represents the type of the season.

- **StartTime**
  - Type: `DateTime?`
  - Description: Represents the start time of the season.

- **EndTime**
  - Type: `DateTime?`
  - Description: Represents the end time of the season.

- **IsActive**
  - Type: `bool?`
  - Description: Indicates whether the season is currently active.

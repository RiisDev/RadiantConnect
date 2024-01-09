## Penalty Record

The `Penalty` record represents penalty-related information in the RadiantConnect network PVP endpoints.

### Properties

- **Subject**
  - Type: `string`
  - Description: Represents the subject associated with the penalty.

- **Penalties**
  - Type: `IReadOnlyList<object>`
  - Description: Represents a list of penalties associated with the subject. The exact structure of each penalty object may vary.

- **Version**
  - Type: `long?`
  - Description: Represents the version of the penalty information.

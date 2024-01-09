## Request Record

The `Request` record represents a request in the RadiantConnect network.

### Properties

#### `GameName`

- Type: `string`
- Description: Represents the game name associated with the request.

#### `GameTag`

- Type: `string`
- Description: Represents the game tag associated with the request.

#### `Name`

- Type: `string`
- Description: Represents the name associated with the request.

#### `Note`

- Type: `string`
- Description: Represents a note associated with the request.

#### `Pid`

- Type: `string`
- Description: Represents the Pid associated with the request.

#### `Puuid`

- Type: `string`
- Description: Represents the Puuid associated with the request.

#### `Region`

- Type: `string`
- Description: Represents the region associated with the request.

#### `Subscription`

- Type: `string`
- Description: Represents the subscription associated with the request.

## InternalRequests Record

The `InternalRequests` record represents a list of internal requests in the RadiantConnect network.

### Properties

#### `Requests`

- Type: `IReadOnlyList<Request>`
- Description: Represents a list of requests.

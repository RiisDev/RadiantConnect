# Authentication Methods


# Authentication Class Methods

This document provides an overview of the methods available in the `Authentication` class, along with usage examples and descriptions. (Auto Generated, please report any issues)

## Methods

### `AuthenticateWithSSID`

```csharp
public async Task<RSOAuth?> AuthenticateWithSSID(string ssid)
```

- **Description**: Authenticates a user using an SSID.
- **Parameters**:
  - `ssid`: The SSID token.
- **Returns**: An `RSOAuth` object containing authentication data.
- **Usage**:
  ```csharp
  RSOAuth? authData = await authentication.AuthenticateWithSSID("ssidToken");
  ```

### `AuthenticateWithQr`

```csharp
public async Task<RSOAuth?> AuthenticateWithQr(CountryCode countryCode, bool returnLoginUrl = false)
```

- **Description**: Authenticates a user using a QR code.
- **Parameters**:
  - `countryCode`: The country code for the authentication.
  - `returnLoginUrl`: If true, returns the login URL.
- **Returns**: An `RSOAuth` object containing authentication data.
- **Usage**:
  ```csharp
  RSOAuth? authData = await authentication.AuthenticateWithQr(CountryCode.NA, true);
  ```

### `AuthenticateWithDriver`

```csharp
public async Task<RSOAuth?> AuthenticateWithDriver(string username, string password, DriverSettings? driverSettings = null)
```

- **Description**: Authenticates a user using a web driver.
- **Parameters**:
  - `username`: The username of the user.
  - `password`: The password of the user.
  - `driverSettings`: Optional settings for the driver.
- **Returns**: An `RSOAuth` object containing authentication data.
- **Usage**:
  ```csharp
  DriverSettings settings = new DriverSettings("msedge", "path/to/msedge", false, true);
  RSOAuth? authData = await authentication.AuthenticateWithDriver("user", "pass", settings);
  ```

### `GetCachedCookies`

```csharp
public async Task<IReadOnlyList<Cookie>?> GetCachedCookies()
```

- **Description**: Retrieves cached cookies.
- **Returns**: A list of cookies if available.
- **Usage**:
  ```csharp
  IReadOnlyList<Cookie>? cookies = await authentication.GetCachedCookies();
  ```

### `GetSsidFromDriverCache`

```csharp
public async Task<string?> GetSsidFromDriverCache()
```

- **Description**: Retrieves the SSID from the driver cache.
- **Returns**: The SSID if available.
- **Usage**:
  ```csharp
  string? ssid = await authentication.GetSsidFromDriverCache();
  ```

### `Logout`

```csharp
public async Task Logout()
```

- **Description**: Logs out the user, from the web driver.
- **Usage**:
  ```csharp
  await authentication.Logout();
  ```

## Obsolete Methods

### `PerformDriverCacheRequest`

- **Description**: This method is obsolete and should not be used. Use `AuthenticateWithSSID` instead.


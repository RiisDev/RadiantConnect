# Authentication Methods

This document provides an overview of the methods available in the `Authentication` class, along with usage examples and descriptions. (Auto Generated, please report any issues)

## AuthenticateWithSSID
It was noted during testing that the CLID paremeter and cookie is **required** to authenticate (Notably EU regions), in NA you should be fine without.
```csharp
public async Task<RSOAuth?> AuthenticateWithSSID(string ssid, string clid? = "")
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` | `SSID Token`  |
| `string` | `CLID Token` (OPTIONAL) |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<RSOAuth?>` (RSOAuth Record) | A record containg all RSO Tokens and User Config |

## AuthenticateWithQr
Note with QR, it has a built in display to show the user the QR code generated. though it offers an `event` if you want to handle the QR yourself.
*Event: OnUrlBuilt*
```csharp
public enum CountryCode
{
    NA,
    KR,
    JP,
    CN,
    TW,
    EUW,
    RU,
    TR,
    TH,
    VN,
    ID,
    MY,
    EUN,
    BR,
}
public async Task<RSOAuth?> AuthenticateWithQr(Authentication.CountryCode countryCode, bool returnLoginUrl = false)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` | `SSID Token`  |
| `bool` | `false` (OPTIONAL) |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<RSOAuth?>` (RSOAuth Record) | A record containg all RSO Tokens and User Config |


## AuthenticateWithDriver
It was noted during testing it takes approximately 5-15 seconds to complete, high success (95%) with Edge browser signed in on a Windows 11/10 Machine, without a VPN 
```csharp
public async Task<RSOAuth?> AuthenticateWithDriver(string username, string password, DriverSettings? driverSettings = null)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` | `username`  |
| `string` | `password`  |
| `DriverSettings` | [DriverSettings](https://irisapp.ca/RadiantConnect/DataTypes/Authentication/DriverSettings/) |
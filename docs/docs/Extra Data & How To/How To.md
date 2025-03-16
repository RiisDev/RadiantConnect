# How To

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

## Using the Initiator Class

The `Initiator` class in RadiantConnect is used to set up and manage connections to the Valorant client. Below are examples of how to use the different constructors available in the `Initiator` class.

### Constructor: `Initiator(RSOAuth rsoAuth)`

This constructor initializes the `Initiator` with an `RSOAuth` object, which is used for authentication.

**Use Case**: Use this constructor when you have an `RSOAuth` object ready for authentication.

**Different ways to gather an RSOAuth: [Authentication](https://irisapp.ca/RadiantConnect/Extra%20Data%20%26%20How%20To/How%20To/)**

```csharp
RSOAuth rsoAuth = new RSOAuth();
Initiator initiator = new Initiator(rsoAuth);

// Access client data
string userId = initiator.Client.UserId;
Console.WriteLine($"User ID: {userId}");
```

### Constructor: `Initiator(bool ignoreVpn = true)`

This constructor initializes the `Initiator` and checks if the Valorant client is ready. It can optionally ignore VPN checks.

**Use Case**: Use this constructor when you want to initialize without an `RSOAuth` object and optionally bypass VPN checks.

**Note: To use this method Valorant must be running**

```csharp
try
{
    Initiator initiator = new Initiator(ignoreVpn: false);
    Console.WriteLine("Initiator initialized successfully.");
}
catch (TimeoutException ex)
{
    Console.WriteLine($"Initialization failed: {ex.Message}");
}
catch (RadiantConnectException ex)
{
    Console.WriteLine($"VPN detected: {ex.Message}");
}
```

### Checking if Valorant is Running

To check if Valorant is currently running, you can use the `ClientIsReady` method.

```csharp
bool isClientReady = Initiator.ClientIsReady();
Console.WriteLine($"Is Valorant Client Ready: {isClientReady}");
```

## Local HttpMethods

```csharp
public enum HttpMethod
{
    Get,
    Post, 
    Put, 
    Delete,
    Patch,
    Options,
    Head
}
```

## Notes

- Ensure you have the necessary permissions and configurations set up for using these methods.
- The examples provided are for illustrative purposes and may require adjustments based on your specific use case and environment.
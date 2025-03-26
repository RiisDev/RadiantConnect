# ValorantNet Usage

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

## Constructing & Using ValorantNet

To use `ValorantNet` you must first use the [Initiator](https://irisapp.ca/RadiantConnect/Extra%20Data%20%26%20How%20To/How%20To/#using-the-initiator-class) to build the client data 

## Access ValorantNet and it's output

Accessing `ValorantNet` and it's output it straight forward for the client to use

```csharp

Initiator initiator = new (rsoData); // Or your desired Initiator builder from above
ValorantNet valorantNet = initiator.ExternalSystem.Net;

valorantNet.OnLog += (log) => Console.WriteLine(log); // Add a event handler to it's output
```

## Getting Base Urls

When you build Initiator it will also generate all the base urls to use for the endpoints, accessed by the following
```csharp
string glzUrl = initiator.Client.GlzUrl; // https://glz-na-1.na.a.pvp.net
string pdUrl = initiator.Client.PdUrl; // https://pd.na.a.pvp.net
string sharedUrl = initiator.Client.SharedUrl; // https://shared.na.a.pvp.net
string userId = initiator.Client.UserId; // ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc
LogService.ClientData.ShardType shard =  initiator.Client.Shard; // na, eu, lam
```

## Using HttpMethods

Each Http method type is available through ValorantNet in 2 forms, either with a object data return or raw http body output.

For example, here is how the default methods are written for a better understanding.

```csharp
public async Task PostAsync;
public async Task<T?> PostAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null);
```

### GetAsync
```csharp
await valorantNet.GetAsync(pdUrl, "/contract-definitions/v3/item-upgrades"); // Will return the raw json of the endpoint
await valorantNet.GetAsync<ItemUpgrade>(pdUrl, "/contract-definitions/v3/item-upgrades"); // With return an object type ItemUpgrade 
```

### PostAsync
```csharp
JsonContent jsonContent = JsonContent.Create(new NameValueCollection() { { "ready", ready.ToString() } });

await valorantNet.PostAsync(Url, $"parties/v1/parties/{partyId}/members/{userId}/setReady", jsonContent);
await valorantNet.PostAsync<PartySetReady>(Url, $"parties/v1/parties/{partyId}/members/{userId}/setReady", jsonContent);
```

### PutAsync
```csharp
await valorantNet.PutAsync(Url, "name-service/v2/players", new StringContent($"[\"{userId}\"]"));
await valorantNet.PutAsync<NameService>(Url, "name-service/v2/players", new StringContent($"[\"{userId}\"]"));
```

### DeleteAsync
```csharp
await valorantNet.DeleteAsync(Url, $"parties/v1/players/{puuid}");
```

### PatchAsync
I currently do not have an example endpoint using PatchAsync, feel free to create a [Pull Request](https://github.com/RiisDev/RadiantConnect/pulls) or [Issue](https://github.com/RiisDev/RadiantConnect/issues) if you have one

### HeadAsync
I currently do not have an example endpoint using HeadAsync, feel free to create a [Pull Request](https://github.com/RiisDev/RadiantConnect/pulls) or [Issue](https://github.com/RiisDev/RadiantConnect/issues) if you have one

### OptionsAsync
I currently do not have an example endpoint using OptionsAsync, feel free to create a [Pull Request](https://github.com/RiisDev/RadiantConnect/pulls) or [Issue](https://github.com/RiisDev/RadiantConnect/issues) if you have one

## Raw Http Request
Do not use this method unless you know what you're doing, note when specifying `customHeaders` you **must** declare your own authentication headers
```csharp
await CreateRequest(HttpMethod httpMethod, string baseUrl, string endPoint, HttpContent? content = null, bool outputDebugData = false, Dictionary<string, string>? customHeaders = null);
```
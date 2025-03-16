# Samples of different code types

## Logging in

### Authenticate with CustomDriver
Note while using driver, it cannot run headlessly, it must have a windows interface, it also does not handle captchas well at all
```csharp
RSOAuth? data = await auth.AuthenticateWithDriver(
    username: "<Username>", 
    password: "<Password>", 
    driverSettings: new DriverSettings(
        ProcessName: "msedge",
        BrowserExecutable: @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
        KillBrowser: false,
        CacheCookies: true
    ));
```

### Authenticate with QR Code
There are 2 ways to run the QR code authentication method, first way you can have it display a QR code to the user, or you can recieve the QR Url, and you can display it yourself.
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

RSOAuth? data = await auth.AuthenticateWithQr(
    countryCode: Authentication.CountryCode.NA, 
    returnLoginUrl: false
);
```

### Authenticate with SSID
As long as you provide the SSID string, RadiantConnect will do the rest.
```csharp
RSOAuth? data = await auth.AuthenticateWithSSID("<SSID_TOKEN>");
```

## Using Initiator 

### Using RSO Auth
```csharp
RSOAuth? data = await auth.AuthenticateWithSSID("<SSID_TOKEN>");
Initiator init = new(data);
```

### Using Valorant Client
Note, this will not work a VPN connected as it messes with proxying!
Note P2, Valorant must be running, if it isn't detected in 45 seconds of starting it will throw **TimeoutException**
```csharp
Initiator init = new();
```

## XMPP

Please check the TCP & XMPP Section!

## Client Events
Note client events cannot be remote and only works locally.
```csharp
Initiator Init = new Initiator();

// Hook into Queue events
Init.GameEvents.Queue.OnEnteredQueue += _ => {
    Debug.WriteLine("Queue Entered");
};

Init.GameEvents.Queue.OnLeftQueue += _ => {
    Debug.WriteLine("Queue Left");
};

Init.GameEvents.Queue.OnQueueChanged += queueChangeType => {
    Debug.WriteLine($"Queue Changed to: {queueChangeType}");
};
```
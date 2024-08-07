# Image Recognition *Beta*

!! danger
	This is still experimental and needs testings and fixes

## This will be rewritten but here is a sample codeblock

```csharp
using RadiantConnect.ImageRecognition;
using RadiantConnect.ImageRecognition.Internals;

ImageRecognition recognition = new();

KillFeedConfig killFeed = new(
	CheckKilled: true,
	CheckAssists: true,
	CheckWasKilled: true
);

Config config = new(
	KillFeedConfig: killFeed,
	SpikePlanted: true
);

recognition.OnSpikeHandlerCreated += (handler) =>
{
	handler.OnSpikeActive += () => Console.WriteLine(value: "Spike Activated");
	handler.OnSpikeDeActive += () => Console.WriteLine(value: "Spike DeActivated");
};

recognition.OnKillFeedHandlerCreated += (handler) =>
{
	handler.OnKill += () => Console.WriteLine(value: "Kill Detected");
	handler.OnAssist += () => Console.WriteLine(value: "Assist Detected");
	handler.OnDeath += () => Console.WriteLine(value: "Death Detected");
};

recognition.Initiator(config: config);

while (true) Console.ReadLine();
```
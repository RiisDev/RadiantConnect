# Image Recognition *Beta*

!!! Danger 
    This is still experimental and needs testing and fixes.

## Overview

The Image Recognition module in RadiantConnect is designed to detect in-game events such as kills, assists, deaths, and spike activations. This document provides a setup guide and a sample code block to help you get started.

## Setup Instructions

1. **Configuration**: Configure the `KillFeedConfig` and `Config` objects to specify which events you want to detect.

2. **Event Handlers**: Attach event handlers to respond to detected events.

## Sample Code

Below is a sample code block demonstrating how to set up and use the Image Recognition module:

```csharp
using RadiantConnect.ImageRecognition;
using RadiantConnect.ImageRecognition.Internals;

// Initialize the ImageRecognition object
ImageRecognition recognition = new();

// Configure the KillFeed settings
KillFeedConfig killFeed = new(
    CheckKilled: true,
    CheckAssists: true,
    CheckWasKilled: true
);

// Create the main configuration object
Config config = new(
    KillFeedConfig: killFeed,
    SpikePlanted: true
);

// Attach event handlers for spike events
recognition.OnSpikeHandlerCreated += (handler) =>
{
    handler.OnSpikeActive += () => Console.WriteLine("Spike Activated");
    handler.OnSpikeDeActive += () => Console.WriteLine("Spike DeActivated");
};

// Attach event handlers for kill feed events
recognition.OnKillFeedHandlerCreated += (handler) =>
{
    handler.OnKill += () => Console.WriteLine("Kill Detected");
    handler.OnAssist += () => Console.WriteLine("Assist Detected");
    handler.OnDeath += () => Console.WriteLine("Death Detected");
};

// Start the recognition process with the specified configuration
recognition.Initiator(config: config);

// Keep the application running to listen for events
while (true) Console.ReadLine();
```

## Notes

- The `Initiator` method is marked as obsolete and may not work correctly. Ensure you test thoroughly and apply necessary fixes.
- This setup is intended for beta testing and may require adjustments based on your specific use case and environment.
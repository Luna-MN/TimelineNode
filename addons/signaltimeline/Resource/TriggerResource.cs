using Godot;
using System;

public partial class TriggerResource : Resource
{
    public Godot.Collections.Dictionary<string, SignalResource> Signals = new();
    public string name;
    
    public event Action<SignalResource> SignalAdded;

    public void AddSignal(string signalName, SignalResource signal)
    {
        Signals[signalName] = signal;
        SignalAdded?.Invoke(signal);
    }
}
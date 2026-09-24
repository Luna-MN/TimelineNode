using Godot;
using System;

public partial class TriggerResource : Resource
{
    public Godot.Collections.Dictionary<string, SignalResource> Signals = new();
    public string name;
}
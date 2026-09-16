using Godot;
using System;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class TimelineResource : Resource
{
    [Export]
    public Dictionary<string, TriggerResource> Triggers = new();
    [Export]
    public Dictionary<string, SignalResource> Signals = new();
}

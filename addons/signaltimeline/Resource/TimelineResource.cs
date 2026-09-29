using Godot;
using System;
using Godot.Collections;
using TimeLinePlugin.addons.signaltimeline.Dock;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

[Tool]
[GlobalClass]
public partial class TimelineResource : Resource
{
    [Export]
    public Dictionary<string, TriggerResource> Triggers = new();
    [Export]
    public Dictionary<string, SignalResource> Signals = new();
    [Export]
    public float Duration = 10.0f;
}

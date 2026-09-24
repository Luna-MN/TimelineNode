using Godot;
using System;
using Godot.Collections;

public partial class SignalResource : Resource
{
    public Dictionary<string, Variant> args = new();
}

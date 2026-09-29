using Godot;
using System;
using System.Collections.Generic;

public partial class TriggerResource : Resource
{
    public Godot.Collections.Dictionary<string, SignalResource> Signals = new();
    public string name;
    
    public event Action<SignalResource> SignalAdded;
    public List<float> ExecutionTimes;

    public void AddSignal(string signalName, SignalResource signal){
        Signals[signalName] = signal;

        var arguments = new Godot.Collections.Array()
        {
            signal.args
        };

        AddUserSignal(name, arguments);
    }

    public void SendSignal(string name, SignalResource signal)
    {
        
    }

    public void SendSignals()
    {
        foreach (var signalPair in Signals)
        {
            SendSignal(signalPair.Key, signalPair.Value);
        }

    }
    
}
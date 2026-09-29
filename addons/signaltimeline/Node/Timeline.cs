using Godot;
using System;
[Tool]
[GlobalClass]
[Icon("res://addons/signaltimeline/Node/Timeline.svg")]

public partial class Timeline : Timer
{
    [Export] public TimelineResource TimelineResource { get; set; }
    public bool TimelineStarted { get; set; }
    public override void _Ready()
    {
    }

    public virtual void StartTimeline()
    {
        WaitTime = TimelineResource.Duration;
        OneShot = true;
        Start();
    }
    public override void _Process(double delta)
    {
        if (!TimelineStarted)
        {
            return;
        }
        foreach (var trigger in TimelineResource.Triggers.Values)
        {
            if (TimeLeft <= trigger.ExecutionTimes[0])
            {
                trigger.SendSignals();
                trigger.ExecutionTimes.RemoveAt(0);
            }
        }
        base._Process(delta);
    }
}

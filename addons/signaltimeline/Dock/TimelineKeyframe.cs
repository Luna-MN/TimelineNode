using Godot;

[Tool]
public partial class TimelineKeyframe : Button
{
    [Signal]
    public delegate void KeyPressedEventHandler(float time);

    [Export] public float Time { get; set; }

    public override void _Ready()
    {
        Pressed += () => EmitSignal(SignalName.KeyPressed, Time);
    }
}

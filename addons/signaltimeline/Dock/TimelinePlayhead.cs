using Godot;

[Tool]
public partial class TimelinePlayhead : Control
{
    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public void SetHeight(float height)
    {
        Size = new Vector2(16, height);
        var line = GetNode<ColorRect>("%Line");
        line.Position = new Vector2(7, 8);
        line.Size = new Vector2(2, Mathf.Max(0, height - 8));
    }
}

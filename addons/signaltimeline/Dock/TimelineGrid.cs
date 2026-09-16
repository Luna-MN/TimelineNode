using Godot;

[Tool]
public partial class TimelineGrid : Control
{
    [Export] public float PixelsPerSecond { get; set; } = 92.0f;
    [Export] public float Duration { get; set; } = 10.0f;
    [Export] public int Subdivisions { get; set; } = 4;

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        Resized += QueueRedraw;
        QueueRedraw();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationThemeChanged ||
            what == NotificationResized ||
            what == NotificationEnterTree)
            QueueRedraw();
    }

    public override void _Draw()
    {
        if (PixelsPerSecond <= 0 || Subdivisions <= 0)
            return;

        Color major = new("#3b2e42");
        Color minor = new("#241b2a");
        Color afterEnd = new(0, 0, 0, 0.20f);

        float step = PixelsPerSecond / Subdivisions;
        int count = Mathf.CeilToInt(Size.X / step) + 1;

        for (int i = 0; i <= count; i++)
        {
            float x = i * step;
            bool isMajor = i % Subdivisions == 0;
            DrawLine(new Vector2(x, 0), new Vector2(x, Size.Y),
                isMajor ? major : minor, 1.0f);
        }

        float endX = Duration * PixelsPerSecond;
        if (endX < Size.X)
            DrawRect(new Rect2(endX, 0, Size.X - endX, Size.Y), afterEnd);
    }
}

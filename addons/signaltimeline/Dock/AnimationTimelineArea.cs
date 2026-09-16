using Godot;
using System.Collections.Generic;

[Tool]
public partial class AnimationTimelineArea : Control
{
    [Signal] public delegate void PlayheadChangedEventHandler(float time);

    [Export] public float PixelsPerSecond { get; set; } = 92.0f;
    [Export] public float Duration { get; set; } = 10.0f;
    [Export] public float PlayheadTime { get; set; } = 0.0f;

    private TimelineGrid _grid;
    private TimelinePlayhead _playhead;
    private Control _keyframeLayer;
    private Control _timelineViewport;
    private Control _rulerViewport;
    private Control _rulerContent;
    [Export]
    private PackedScene _keyframeScene;
    [Export]
    private PackedScene _trackInfoScene;
    [Export]
    private PackedScene _trackScene;
    private Button _addTrackButton;

    private readonly float[] _keys = { 0.0f, 1.25f, 3.0f, 4.5f, 7.2f };
    private readonly List<Node> _rulerNodes = new();
    private readonly List<TimelineKeyframe> _keyNodes = new();

    private bool _movingPlayhead = false;
    private Vector2 _mousePos;
    public override void _Ready()
    {
        _grid = GetNode<TimelineGrid>("%Grid");
        _playhead = GetNode<TimelinePlayhead>("%Playhead");
        _keyframeLayer = GetNode<Control>("%KeyframeLayer");
        _timelineViewport = GetNode<Control>("%TimelineViewport");
        _rulerViewport = GetNode<Control>("%RulerViewport");
        _rulerContent = GetNode<Control>("%RulerContent");
        _addTrackButton = GetNode<Button>("%AddTrack");

        _timelineViewport.GuiInput += OnTimelineGuiInput;
        _timelineViewport.Resized += RefreshLayout;
        _rulerViewport.Resized += RefreshLayout;
        _addTrackButton.ButtonUp += OnAddTrackButtonPressed;

        CallDeferred(MethodName.RefreshLayout);
    }

    public override void _Process(double delta)
    {
        if (_movingPlayhead)
        {
            MovePlayhead();
        }
    }

    public void SetPlayhead(float time)
    {
        PlayheadTime = Mathf.Clamp(time, 0.0f, Duration);
        float x = PlayheadTime * PixelsPerSecond;
        _playhead.Position = new Vector2(x - 8.0f, 0);
        _playhead.SetHeight(_timelineViewport.Size.Y);
        EmitSignal(SignalName.PlayheadChanged, PlayheadTime);
    }

    public void SetZoom(float pixelsPerSecond)
    {
        PixelsPerSecond = Mathf.Max(12.0f, pixelsPerSecond);
        RefreshLayout();
    }

    public void SetDuration(float duration)
    {
        Duration = Mathf.Max(0.001f, duration);
        RefreshLayout();
    }

    private void OnTimelineGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouse &&
            mouse.ButtonIndex == MouseButton.Left)
        {

            if (mouse.Pressed)
            {
                _movingPlayhead = true;
            }
            else
            {
                _movingPlayhead = false;
            }
            _mousePos = mouse.Position;
        }

        if (inputEvent is InputEventMouseMotion mousePos)
        {
            _mousePos = mousePos.Position;
        }
    }

    private void MovePlayhead()
    {
        PlayheadTime = Mathf.Clamp(_mousePos.X / PixelsPerSecond, 0.0f, Duration);
        EmitSignal(SignalName.PlayheadChanged, PlayheadTime);
        SetPlayhead(PlayheadTime);
        AcceptEvent();
    }
    private void RefreshLayout()
    {
        if (!IsNodeReady())
            return;

        _grid.PixelsPerSecond = PixelsPerSecond;
        _grid.Duration = Duration;
        _grid.QueueRedraw();

        BuildRuler();
        BuildKeys();
        SetPlayhead(PlayheadTime);
    }

    private void BuildRuler()
    {
        foreach (var n in _rulerNodes)
            n.QueueFree();
        _rulerNodes.Clear();

        float width = _rulerViewport.Size.X;
        if (width <= 1)
            return;

        int secondsVisible = Mathf.CeilToInt(width / PixelsPerSecond) + 1;

        for (int second = 0; second <= secondsVisible; second++)
        {
            float x = second * PixelsPerSecond;

            var line = new ColorRect
            {
                Color = new Color("#413348"),
                Position = new Vector2(x, 0),
                Size = new Vector2(1, 30),
                MouseFilter = MouseFilterEnum.Ignore
            };
            _rulerContent.AddChild(line);
            _rulerNodes.Add(line);

            var label = new Label
            {
                Text = second.ToString(),
                Position = new Vector2(x + 5, 2),
                Size = new Vector2(42, 24),
                MouseFilter = MouseFilterEnum.Ignore
            };
            label.AddThemeColorOverride("font_color", new Color("#9e95a3"));
            _rulerContent.AddChild(label);
            _rulerNodes.Add(label);
        }
    }

    private void BuildKeys()
    {
        foreach (var n in _keyNodes)
            n.QueueFree();
        _keyNodes.Clear();

        if (_keyframeScene == null)
            return;

        foreach (float t in _keys)
        {
            var key = _keyframeScene.Instantiate<TimelineKeyframe>();
            key.Time = t;
            key.Position = new Vector2(t * PixelsPerSecond - 5, 9);
            key.KeyPressed += SetPlayhead;
            _keyframeLayer.AddChild(key);
            _keyNodes.Add(key);
        }
    }

    private void OnAddTrackButtonPressed()
    {
        var TrackRow = _trackInfoScene.Instantiate<TimelineTrackRow>();
        var sidebar = GetNode<VBoxContainer>("%Sidebar");
        sidebar.AddChild(TrackRow);
        sidebar.MoveChild(TrackRow, sidebar.GetChildCount() - 2);
        var TrackLane = _trackScene.Instantiate<AnimationTrackLane>();
        var Tracks = GetNode<VBoxContainer>("%Tracks");
        TrackLane.CustomMinimumSize = new Vector2(TrackLane.Size.X, TrackRow.Size.Y);
        Tracks.AddChild(TrackLane);
        Tracks.MoveChild(TrackLane, Tracks.GetChildCount() - 1);
        TrackRow.Connect(
            TimelineTrackRow.SignalName.TrackDeleted,
            Callable.From(TrackLane.QueueFree)
        );

    }
}

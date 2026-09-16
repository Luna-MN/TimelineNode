using Godot;
using System;
using System.Collections.Generic;
[Tool]
public partial class TimelineEditor : Control
{
    private const float SidebarWidth = 190.0f;
    private const float RulerHeight = 34.0f;
    private const float RowHeight = 28.0f;

    [Export] public float PixelsPerSecond { get; set; } = 100.0f;
    [Export] public float Duration { get; set; } = 15.0f;
    [Export] public float PlayheadTime { get; set; } = 0.0f;
    private Callable _playheadChanged;
    private AnimationTimelineArea _timelineCanvas;
    private HSlider _zoom;
    // private LineEdit _filter;
    private Label _timeLabel;
    private Button _playButton;
    private bool _playing;

    private readonly List<float> _keys = new() { 0.0f, 1.25f, 3.0f, 4.5f, 7.2f, 10.0f };
    
    public override void _Ready()
    {
        _timelineCanvas = GetNode<AnimationTimelineArea>("%TimelineCanvas");
        _playheadChanged = new Callable(this, nameof(OnTimelinePlayheadChanged));
        _timelineCanvas.Connect("PlayheadChanged", _playheadChanged);
        
        _zoom = GetNode<HSlider>("%ZoomSlider");
        // _filter = GetNode<LineEdit>("%FilterTracks");
        _timeLabel = GetNode<Label>("%TimeLabel");
        _playButton = GetNode<Button>("%PlayButton");
        
        _timelineCanvas.GuiInput += OnTimelineGuiInput;
        _zoom.ValueChanged += OnZoomChanged;
        // _filter.TextChanged += OnFilterChanged;
        _playButton.ButtonUp += OnPlayPressed;
        GetNode<Button>("%StopButton").ButtonUp += OnStopPressed;
        GetNode<Button>("%RewindButton").ButtonUp += () => SetPlayhead(0.0f);

        SetProcess(true);
        UpdateTimeLabel();
        _timelineCanvas.QueueRedraw();
    }

    public override void _Process(double delta)
    {
        if (!_playing)
            return;

        PlayheadTime += (float)delta;
        if (PlayheadTime > Duration)
            PlayheadTime = 0.0f;

        UpdateTimeLabel();
        _timelineCanvas.QueueRedraw();
        SetPlayhead(PlayheadTime);
    }

    private void OnPlayPressed()
    {
        _playing = !_playing;
        _playButton.Text = _playing ? "❚❚" : "▶";
    }

    private void OnStopPressed()
    {
        _playing = false;
        _playButton.Text = "▶";
        SetPlayhead(0.0f);
    }

    private void OnZoomChanged(double value)
    {
        PixelsPerSecond = 45.0f + (float)value * 2.2f;
        _timelineCanvas.QueueRedraw();
    }

    private void OnFilterChanged(string text)
    {
        var row = GetNode<Control>("%TrackRow");
        row.Visible = string.IsNullOrWhiteSpace(text) ||
                      "clip_text".Contains(text, StringComparison.OrdinalIgnoreCase);
    }

    private void OnTimelineGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouse &&
            mouse.ButtonIndex == MouseButton.Left &&
            mouse.Pressed)
        {
            float x = mouse.Position.X - SidebarWidth;
            if (x >= 0)
                SetPlayhead(Mathf.Clamp(x / PixelsPerSecond, 0.0f, Duration));
        }
    }

    private void SetPlayhead(float time)
    {
        PlayheadTime = time;
        UpdateTimeLabel();
        _timelineCanvas.QueueRedraw();
        _timelineCanvas.SetPlayhead(time);
    }

    private void UpdateTimeLabel()
    {
        if (_timeLabel != null)
            _timeLabel.Text = PlayheadTime.ToString("0.0");
    }

    private void OnTimelinePlayheadChanged(float time)
    {
        PlayheadTime = time;
        _timeLabel.Text = time.ToString("0.0");
    }
}

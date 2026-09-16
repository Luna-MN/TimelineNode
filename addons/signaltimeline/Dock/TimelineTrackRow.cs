using Godot;

[Tool]
public partial class TimelineTrackRow : PanelContainer
{
    [Signal] public delegate void EnabledChangedEventHandler(bool enabled);
    [Signal] public delegate void TrackDeletedEventHandler();

    [Export] public string TrackName { get; set; } = "clip_text";
    [Export] public bool TrackEnabled { get; set; } = true;
    [Export] public Color TypeIconColor { get; set; } = Colors.White;

    private Label _trackNameLabel;
    private LineEdit _trackNameEdit;
    private Label _typeIcon;
    private Button _DeleteButton;

    private PopupPanel _typeColorPopup;
    private ColorPicker _typeColorPicker;

    public override void _Ready()
    {
        _trackNameLabel = GetNode<Label>("%TrackNameLabel");
        _trackNameLabel.Text = TrackName;
        CreateTrackNameEditor();
        _trackNameLabel.GuiInput += OnTrackNameLabelGuiInput;

        var cb = GetNode<CheckBox>("%EnabledCheck");
        cb.ButtonPressed = TrackEnabled;
        cb.Toggled += enabled =>
        {
            TrackEnabled = enabled;
            EmitSignal(SignalName.EnabledChanged, enabled);
        };

        _typeIcon = GetNode<Label>("%TypeIcon");
        _typeIcon.Modulate = TypeIconColor;
        _typeIcon.MouseFilter = MouseFilterEnum.Stop;
        _typeIcon.GuiInput += OnTypeIconGuiInput;

        CreateTypeIconColorChooser();
        
        _DeleteButton = GetNode<Button>("%Delete");
        _DeleteButton.Icon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Remove", "EditorIcons");
        _DeleteButton.ButtonUp += OnDeleteButtonPressed;
    }

    private void CreateTrackNameEditor()
    {
        _trackNameEdit = new LineEdit
        {
            Text = TrackName,
            Visible = false,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        var parent = _trackNameLabel.GetParent();
        parent.AddChild(_trackNameEdit);
        parent.MoveChild(_trackNameEdit, _trackNameLabel.GetIndex() + 1);

        _trackNameEdit.TextSubmitted += _ => FinishEditingTrackName();
        _trackNameEdit.FocusExited += FinishEditingTrackName;

        _trackNameEdit.GuiInput += inputEvent =>
        {
            if (inputEvent is InputEventKey keyEvent &&
                keyEvent.Pressed &&
                keyEvent.Keycode == Key.Escape)
            {
                CancelEditingTrackName();
            }
        };
    }

    private void CreateTypeIconColorChooser()
    {
        _typeColorPopup = new PopupPanel
        {
            MinSize = new Vector2I(360, 420)
        };

        _typeColorPicker = new ColorPicker
        {
            Color = TypeIconColor,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill
        };

        _typeColorPicker.ColorChanged += color =>
        {
            TypeIconColor = color;
            _typeIcon.Modulate = color;
        };

        _typeColorPopup.AddChild(_typeColorPicker);
        AddChild(_typeColorPopup);
    }

    private void OnGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventKey keyEvent &&
            keyEvent.Pressed &&
            keyEvent.Keycode == Key.Delete)
        {
            GetParent<VBoxContainer>().RemoveChild(this);
        }
    }

    private void OnTrackNameLabelGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent &&
            mouseEvent.ButtonIndex == MouseButton.Left &&
            mouseEvent.DoubleClick)
        {
            StartEditingTrackName();
        }
    }

    private void OnTypeIconGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent &&
            mouseEvent.ButtonIndex == MouseButton.Left &&
            mouseEvent.DoubleClick)
        {
            OpenTypeIconColorChooser();
            AcceptEvent();
        }
    }

    private void OpenTypeIconColorChooser()
    {
        _typeColorPicker.Color = TypeIconColor;
        _typeColorPopup.PopupCentered(new Vector2I(360, 420));
    }

    private void StartEditingTrackName()
    {
        _trackNameEdit.Text = TrackName;

        _trackNameLabel.Visible = false;
        _trackNameEdit.Visible = true;

        _trackNameEdit.GrabFocus();
        _trackNameEdit.SelectAll();
    }

    private void FinishEditingTrackName()
    {
        if (!_trackNameEdit.Visible)
            return;

        TrackName = _trackNameEdit.Text;
        _trackNameLabel.Text = TrackName;

        _trackNameEdit.Visible = false;
        _trackNameLabel.Visible = true;
    }

    private void CancelEditingTrackName()
    {
        _trackNameEdit.Text = TrackName;

        _trackNameEdit.Visible = false;
        _trackNameLabel.Visible = true;
    }

    private void OnDeleteButtonPressed()
    {
        EmitSignal(SignalName.TrackDeleted);
        GetParent<VBoxContainer>().RemoveChild(this);
        QueueFree();
    }
}
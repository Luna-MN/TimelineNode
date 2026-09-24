using Godot;
using System;
using TimeLinePlugin.addons.signaltimeline.Dock;

[Tool]
public partial class AnimationTrackLane : PanelContainer
{
    [Signal]
    public delegate void AddTriggerRequestedEventHandler(float laneX);
    [Export]
    private PackedScene _triggerScene;
    private const int AddTriggerMenuId = 1;

    private PopupMenu _popupMenu;
    private float _lastRightClickLaneX;

    public Callable DeleteTrack;

    public override void _Ready()
    {
        base._Ready();

        DeleteTrack = new Callable(this, nameof(QueueFree));

        // Receive mouse events, but don't automatically consume them.
        MouseFilter = MouseFilterEnum.Pass;

        _popupMenu = new PopupMenu();
        _popupMenu.AddItem("Add Trigger", AddTriggerMenuId);
        _popupMenu.IdPressed += OnPopupMenuIdPressed;
        AddChild(_popupMenu);   
    }


    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);

        if (@event is not InputEventMouseButton mouseButton)
            return;

        // Ignore left clicks and let them continue to parent controls.
        if (mouseButton.ButtonIndex != MouseButton.Right || !mouseButton.Pressed)
            return;

        _lastRightClickLaneX = mouseButton.Position.X;

        Vector2I popupPosition = DisplayServer.MouseGetPosition();
        _popupMenu.Popup(new Rect2I(popupPosition, Vector2I.Zero));

        // Stop only the right-click event.
        AcceptEvent();
    }


    private void OnPopupMenuIdPressed(long id)
    {
        if (id == AddTriggerMenuId)
        {
            if (Globals.Resource == null)
            {
                GD.PushWarning("No TimelineResource set on Globals.Resource; cannot open trigger popup.");
                return;
            }

            float spawnX = _lastRightClickLaneX;

            Globals.Resource.OpenTriggerPopup(this, (triggerName, _) =>
            {
                SpawnTrigger(triggerName, spawnX);
            });
        }
    }
    private void SpawnTrigger(string triggerName, float laneX)
    {
        var trigger = _triggerScene.Instantiate<Control>();
        trigger.Name = triggerName;
        GetNode("%TriggerLayer").AddChild(trigger);
        trigger.Position = new Vector2(laneX, trigger.Position.Y);
    }
    public void SetSize(Vector2 size)
    {
        Size = new Vector2(Size.X, size.Y);
    }

    public void SetPosition(Vector2 position)
    {
        Position = position;
    }
}
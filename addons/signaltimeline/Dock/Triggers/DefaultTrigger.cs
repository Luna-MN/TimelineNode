using Godot;
using System;
using TimeLinePlugin.addons.signaltimeline.Dock;

[Tool]
public partial class DefaultTrigger : ColorRect
{
    public TriggerResource Resource { get; set; }
    private EditorInspector inspector;
    private PopupMenu _popupMenu;
    private const int AddSignalId = 1;
    private const int ShowSignalId = 2;
    
    public override void _Ready()
    {
        Resource = Globals.Resource.GetTriggerResource(Name);
        inspector = EditorInterface.Singleton.GetInspector();
        _popupMenu = new PopupMenu();
        _popupMenu.AddItem("Show Signals", ShowSignalId);
        _popupMenu.IdPressed += OnPopupMenuIdPressed;
        AddChild(_popupMenu);   
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left })
        {
            inspector.Edit(Resource);
        }
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Right })
        {
            Vector2I popupPosition = DisplayServer.MouseGetPosition();
            _popupMenu.Popup(new Rect2I(popupPosition, Vector2I.Zero));
        }
    }
    private void OnPopupMenuIdPressed(long id)
    {
        if (id == ShowSignalId)
        {
            Globals.Resource.OpenSignalsPopup(Resource.name, this, signalName =>
            {
                GD.Print($"Selected signal: {signalName}");
            });
        }

    }
}

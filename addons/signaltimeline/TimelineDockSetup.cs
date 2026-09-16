#if TOOLS
using Godot;

[Tool]
public partial class TimelineDockSetup : EditorPlugin
{
    private EditorDock _dock;
    private EditorDock _signalDock;
    public override void _EnterTree()
    {
        var _dock_scene = GD.Load<PackedScene>("res://addons/signaltimeline/Dock/TimelineEditor.tscn").Instantiate<Control>();

        // Create the dock and add the loaded scene to it.
        _dock = new EditorDock();
        _dock.AddChild(_dock_scene);

        _dock.Title = "Signal Timeline";

        // Note that LeftUl means the left of the editor, upper-left dock.
        _dock.DefaultSlot = EditorDock.DockSlot.Bottom;

        // Allow the dock to be on the left or right of the editor, and to be made floating.
        _dock.AvailableLayouts = EditorDock.DockLayout.Horizontal | EditorDock.DockLayout.Floating;

        AddDock(_dock);
        
        var _signalDock_scene = GD.Load<PackedScene>("res://addons/signaltimeline/Signal Inspector/SignalInspector.tscn").Instantiate<SignalInspector>();

        _signalDock = new EditorDock();
        _signalDock.AddChild(_signalDock_scene);

        _signalDock.Title = "Signal Inspector";

        _signalDock.DefaultSlot = EditorDock.DockSlot.RightUl;
        
        _signalDock.AvailableLayouts = EditorDock.DockLayout.Vertical | EditorDock.DockLayout.Floating;
        
        AddDock(_signalDock);

    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        // Remove the dock.
        RemoveDock(_dock);
        // Erase the control from the memory.
        _dock.QueueFree();
    }
}
#endif
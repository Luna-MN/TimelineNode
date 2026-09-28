using System.Linq;
using Godot;
using Godot.Collections;
using TimeLinePlugin.addons.signaltimeline.Dock;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

[Tool]
public partial class TriggerInspector : Tree
{
	private const int AddSignalButtonId = 1;
	private const int AddSignalContextId = 1;

	private TreeItem _signalsRoot = null!;
	private PopupMenu _contextMenu = null!;
	private TreeItem? _contextTriggerItem;
	public Dictionary<string, TreeItem> Triggers = new();
	public override void _EnterTree()
	{
		Columns = 1;
		HideRoot = false;

		ButtonClicked += OnButtonClicked;
		ItemEdited += OnTreeItemEdited;
		Globals.Resource.AddTrigger += AddTigger;

		_contextMenu = new PopupMenu();
		_contextMenu.AddItem("Add Signal", AddSignalContextId);
		_contextMenu.IdPressed += OnContextMenuIdPressed;
		AddChild(_contextMenu);

		BuildTree();
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true } mouseEvent)
		{
			var item = GetItemAtPosition(mouseEvent.Position);
			if (item == null || !Triggers.Values.Contains(item))
				return;

			_contextTriggerItem = item;

			Vector2I popupPosition = DisplayServer.MouseGetPosition();
			_contextMenu.Popup(new Rect2I(popupPosition, Vector2I.Zero));
		}
	}

	private void OnContextMenuIdPressed(long id)
	{
		if (id != AddSignalContextId || _contextTriggerItem == null)
			return;

		var triggerItem = _contextTriggerItem;
		var pair = Triggers.FirstOrDefault(x => x.Value == triggerItem);
		if (pair.Value == null)
			return;

		var triggerResource = Globals.Resource.GetTriggerResource(pair.Key);
		if (triggerResource == null)
			return;

		Globals.Resource.OpenSignalsPopup(this, (signalName, signalResource) =>
		{
			triggerResource.AddSignal(signalName, signalResource);
		});
	}

	private void AddSignalItem(TreeItem triggerItem, SignalResource signal)
	{
		var signalItem = CreateItem(triggerItem);
		signalItem.SetText(0, signal.name);
	}

	private void BuildTree()
	{
		Clear();

		_signalsRoot = CreateItem();
		_signalsRoot.SetText(0, "Triggers");

		Texture2D addIcon = GetThemeIcon("Add", "EditorIcons");
		var item = CreateItem(_signalsRoot);
		item.SetText(0, "Add Trigger");
		item.AddButton(0, addIcon, AddSignalButtonId, false, "Add Trigger");
	}

	private void OnButtonClicked(TreeItem item, long column, long id, long mouseButtonIndex)
	{
		if (id != AddSignalButtonId)
			return;

		AddTrigger();
	}

	private void AddTigger(TriggerResource trigger)
	{
		TreeItem triggerItem = CreateItem(_signalsRoot);
		triggerItem.SetText(0, trigger.name);
		triggerItem.SetEditable(0, true);

		triggerItem.Select(0);

		trigger.SignalAdded += signal => AddSignalItem(triggerItem, signal);
		CollectionExtensions.TryAdd(Triggers, trigger.name, triggerItem);
	}

	private void AddTrigger()
	{
		Globals.Resource.OpenTriggerPopup(this);

	}

	private void OnTreeItemEdited()
	{
		var item = GetEdited();
		if (!Triggers.Values.Contains(item)) return;
    
		var pair = Triggers.Where(x => x.Value == item);
		if (!Triggers.ContainsKey(item.GetText(0)))
		{
			Triggers.Remove(pair.First().Key);
			Triggers.Add(item.GetText(0), item);
		}
		else
		{
			item.SetText(0, pair.First().Key);
		}
	}
}
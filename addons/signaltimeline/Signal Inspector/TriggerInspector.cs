using Godot;
using System;
using System.Linq;
using Godot.Collections;
using TimeLinePlugin.addons.signaltimeline.Dock;

[Tool]
public partial class TriggerInspector : Tree
{
	private const int AddSignalButtonId = 1;

	private TreeItem _signalsRoot = null!;
	public Dictionary<string, TreeItem> Triggers = new();
	public override void _EnterTree()
	{
		Columns = 1;
		HideRoot = false;

		ButtonClicked += OnButtonClicked;
		ItemEdited += OnTreeItemEdited;
		Globals.Resource.AddTrigger += AddTigger;
		BuildTree();
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
        
		Triggers.Add(trigger.name, triggerItem);
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

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TimeLinePlugin.addons.signaltimeline.Dock;

[Tool]
public partial class SignalInspector : Tree
{
    private const int AddSignalButtonId = 1;

    private TreeItem _signalsRoot = null!;
    public Dictionary<string, TreeItem> Signals = new();
    public override void _EnterTree()
    {
        Columns = 1;
        HideRoot = false;

        ButtonClicked += OnButtonClicked;
        ItemEdited += OnTreeItemEdited;
        Globals.Resource.AddSignal += AddSignal;
        BuildTree();
    }

    private void BuildTree()
    {
        Clear();

        _signalsRoot = CreateItem();
        _signalsRoot.SetText(0, "Signals");

        Texture2D addIcon = GetThemeIcon("Add", "EditorIcons");
        var item = CreateItem(_signalsRoot);
        item.SetText(0, "Add Signal");
        item.AddButton(0, addIcon, AddSignalButtonId, false, "Add Signal");
    }

    private void OnButtonClicked(TreeItem item, long column, long id, long mouseButtonIndex)
    {
        if (id != AddSignalButtonId)
            return;

        AddSignal();
    }
    private void AddSignal(SignalResource signal)
    {
        TreeItem signalItem = CreateItem(_signalsRoot);
        signalItem.SetText(0, signal.name);
        signalItem.SetEditable(0, true);

        signalItem.Select(0);
        
        Signals.Add(signal.name, signalItem);
    }

    private void AddSignal()
    {
        Globals.Resource.OpenSignalsPopup( this);
    }

    private void OnTreeItemEdited()
    {
        var item = GetEdited();
        if (!Signals.Values.Contains(item)) return;
        
        var pair = Signals.Where(x => x.Value == item);
        if (!Signals.ContainsKey(item.GetText(0)))
        {
            Signals.Remove(pair.First().Key);
            Signals.Add(item.GetText(0), item);
        }
        else
        {
            item.SetText(0, pair.First().Key);
        }
    }
}
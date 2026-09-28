using Godot;
using System;
using Godot.Collections;
using TimeLinePlugin.addons.signaltimeline.Dock;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

[Tool]
[GlobalClass]
public partial class TimelineResource : Resource
{
    [Export]
    public Dictionary<string, TriggerResource> Triggers = new();
    [Export]
    public Dictionary<string, SignalResource> Signals = new();

    public event Action<TriggerResource> AddTrigger;
    public event Action<SignalResource> AddSignal;
    public void CreateSignal(string name, Dictionary<string, Variant> args, Variant.Type type)
    {
        args["type"] = (int)type;
        args["name"] = name;

        Signals[name].args = args;
        Signals[name].name = name;

        var arguments = new Godot.Collections.Array()
        {
            args
        };

        AddUserSignal(name, arguments);
    }
        /// <summary>
        /// Opens a popup that lets the user either create a new named signal
        /// or pick an existing signal to reference.
        /// The chosen (or newly created) signal name is returned via <paramref name="onSignalSelected"/>.
        /// </summary>
        public PopupPanel OpenSignalsPopup(Control owner, Action<string, SignalResource>? onSignalSelected = null)
        {
            var popup = new PopupPanel
            {
                MinSize = new Vector2I(320, 320)
            };

            var margin = new MarginContainer();
            margin.AddThemeConstantOverride("margin_left", 8);
            margin.AddThemeConstantOverride("margin_top", 8);
            margin.AddThemeConstantOverride("margin_right", 8);
            margin.AddThemeConstantOverride("margin_bottom", 8);

            var root = new VBoxContainer
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill
            };

            // --- "Create new signal" section ---
            root.AddChild(new Label { Text = "Create new signal" });

            var newRow = new HBoxContainer
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };

            var nameEdit = new LineEdit
            {
                PlaceholderText = "Signal name",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };
            newRow.AddChild(nameEdit);

            var createButton = new Button { Text = "Create" };
            newRow.AddChild(createButton);
            root.AddChild(newRow);

            root.AddChild(new HSeparator());

            // --- "Existing signals" section ---
            root.AddChild(new Label { Text = "Reference existing signal" });

            var scroll = new ScrollContainer
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill
            };

            var list = new VBoxContainer
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };

            if (Signals.Count == 0)
            {
                list.AddChild(new Label { Text = "No signals yet." });
            }
            else
            {
                foreach (var pair in Signals)
                {
                    string signalName = pair.Key;
                    SignalResource signalRes = pair.Value;

                    var button = new Button
                    {
                        Text = BuildSignalText(signalName, signalRes),
                        Alignment = HorizontalAlignment.Left,
                        SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                    };

                    button.Pressed += () =>
                    {
                        onSignalSelected?.Invoke(signalName, signalRes);
                        popup.QueueFree();
                    };

                    list.AddChild(button);
                }
            }

            scroll.AddChild(list);
            root.AddChild(scroll);

            createButton.Pressed += () =>
            {
                string chosen = nameEdit.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(chosen) || Signals.ContainsKey(chosen))
                    return;

                var newResource = new SignalResource { name = chosen };
                Signals[chosen] = newResource;
                AddSignal?.Invoke(newResource);

                onSignalSelected?.Invoke(chosen, newResource);
                popup.QueueFree();
            };

            margin.AddChild(root);
            popup.AddChild(margin);

            owner.AddChild(popup);
            popup.PopupCentered();

            return popup;
        }
    private string BuildSignalText(string signalName, SignalResource signal)
    {
        if (!signal.args.TryGetValue("type", out Variant typeVariant))
            return $"{signalName}()";

        Variant.Type type = (Variant.Type)typeVariant.AsInt32();

        return $"{signalName}({signalName}: {type})";
    }

    /// <summary>
    /// Opens a popup that lets the user either create a new named trigger
    /// or pick an existing trigger to make an instance of.
    /// The chosen (or newly created) trigger name is returned via <paramref name="onTriggerSelected"/>.
    /// </summary>
    public PopupPanel OpenTriggerPopup(Control owner, Action<string, TriggerResource>? onTriggerSelected = null)
    {
        var popup = new PopupPanel
        {
            MinSize = new Vector2I(320, 320)
        };

        var margin = new MarginContainer();
        margin.AddThemeConstantOverride("margin_left", 8);
        margin.AddThemeConstantOverride("margin_top", 8);
        margin.AddThemeConstantOverride("margin_right", 8);
        margin.AddThemeConstantOverride("margin_bottom", 8);

        var root = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };

        // --- "Create new trigger" section ---
        root.AddChild(new Label { Text = "Create new trigger" });

        var newRow = new HBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };

        var nameEdit = new LineEdit
        {
            PlaceholderText = "Trigger name",
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        newRow.AddChild(nameEdit);

        var createButton = new Button { Text = "Create" };
        newRow.AddChild(createButton);
        root.AddChild(newRow);

        root.AddChild(new HSeparator());

        // --- "Existing triggers" section ---
        root.AddChild(new Label { Text = "Instance existing trigger" });

        var scroll = new ScrollContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };

        var list = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };

        if (Triggers.Count == 0)
        {
            list.AddChild(new Label { Text = "No triggers yet." });
        }
        else
        {
            foreach (var pair in Triggers)
            {
                string triggerName = pair.Key;
                TriggerResource triggerRes = pair.Value;

                var button = new Button
                {
                    Text = triggerName,
                    Alignment = HorizontalAlignment.Left,
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                };

                button.Pressed += () =>
                {
                    onTriggerSelected?.Invoke(triggerName, triggerRes);
                    popup.QueueFree();
                };

                list.AddChild(button);
            }
        }

        scroll.AddChild(list);
        root.AddChild(scroll);

        createButton.Pressed += () =>
        {
            string chosen = nameEdit.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(chosen) || Triggers.ContainsKey(chosen))
                return;

            var newResource = new TriggerResource { name = chosen };
            Triggers[chosen] = newResource;
            Globals.Resource.AddTrigger?.Invoke(newResource);

            onTriggerSelected?.Invoke(chosen, newResource);
            popup.QueueFree();
        };

        margin.AddChild(root);
        popup.AddChild(margin);

        owner.AddChild(popup);
        popup.PopupCentered();

        return popup;
    }

    public TriggerResource GetTriggerResource(StringName name)
    {
        return CollectionExtensions.GetValueOrDefault(Triggers, name);
    }
}

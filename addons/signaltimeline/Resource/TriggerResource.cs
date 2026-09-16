using Godot;
using System;

public partial class TriggerResource : Resource
{
    public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Variant>> Signals = new();

    public void CreateSignal(string name, Godot.Collections.Dictionary<string, Variant> args, Variant.Type type)
    {
        args["type"] = (int)type;
        args["name"] = name;

        Signals[name] = args;

        var arguments = new Godot.Collections.Array()
        {
            args
        };

        AddUserSignal(name, arguments);
    }

    public PopupPanel OpenSignalsPopup(Control owner, Action<string>? onSignalSelected = null)
    {
        var popup = new PopupPanel
        {
            MinSize = new Vector2I(320, 300)
        };

        var margin = new MarginContainer();
        margin.AddThemeConstantOverride("margin_left", 8);
        margin.AddThemeConstantOverride("margin_top", 8);
        margin.AddThemeConstantOverride("margin_right", 8);
        margin.AddThemeConstantOverride("margin_bottom", 8);

        var scroll = new ScrollContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };

        var container = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };

        if (Signals.Count == 0)
        {
            container.AddChild(new Label
            {
                Text = "No signals found."
            });
        }
        else
        {
            foreach (var signalPair in Signals)
            {
                string signalName = signalPair.Key;
                Godot.Collections.Dictionary<string, Variant> args = signalPair.Value;

                var button = new Button
                {
                    Text = BuildSignalText(signalName, args),
                    Alignment = HorizontalAlignment.Left,
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                };

                button.Pressed += () =>
                {
                    onSignalSelected?.Invoke(signalName);
                    popup.QueueFree();
                };

                container.AddChild(button);
            }
        }

        scroll.AddChild(container);
        margin.AddChild(scroll);
        popup.AddChild(margin);

        owner.AddChild(popup);
        popup.PopupCentered();

        return popup;
    }

    private string BuildSignalText(string signalName, Godot.Collections.Dictionary<string, Variant> args)
    {
        if (!args.TryGetValue("type", out Variant typeVariant))
            return $"{signalName}()";

        Variant.Type type = (Variant.Type)typeVariant.AsInt32();

        return $"{signalName}({signalName}: {type})";
    }
}
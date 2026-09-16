using Godot;
using System;
[Tool]
public partial class AddRow : Button
{

    public override void _Ready()
    {
        ButtonUp += onAddRowPressed;
    }
    private void onAddRowPressed()
    {
        GD.Print("Meow");
    }
}

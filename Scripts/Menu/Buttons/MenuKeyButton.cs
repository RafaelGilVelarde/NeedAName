using Godot;
using System;

public partial class MenuKeyButton : Node
{
    [Export] StatsScreen Screen;
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsPressed()){
            Screen.ChangeKey(@event);
        }
    }
}

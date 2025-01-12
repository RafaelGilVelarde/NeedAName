using Godot;
using Godot.Collections;
using System;
[GlobalClass]

public partial class ConsumableBase : ItemBase
{
    [Export] public Moves ItemMove;
    public override void Effect(Array<Character> Targets)
    {
    }
}

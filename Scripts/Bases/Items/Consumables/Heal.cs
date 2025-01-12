using Godot;
using Godot.Collections;
using System;
[GlobalClass]
public partial class Heal : ConsumableBase
{
    [Export] int HPHeal;
    public override void Effect(Array<Character> Targets)
    {
        base.Effect(Targets);
        Targets[0].ChangeHP(HPHeal);
    }
}

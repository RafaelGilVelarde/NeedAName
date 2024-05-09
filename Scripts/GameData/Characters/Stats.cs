using Godot;
using System;
[Tool]
[GlobalClass]

public partial class Stats : Resource
{
    [Export]public int MaxHP, HP, Atk, Def, SpAtk, SpDef, Speed;
}

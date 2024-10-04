using Godot;
using System;
[Tool]
[GlobalClass]

public partial class Stats : Resource
{
    [Export]public int Lv,MaxHP, HP, Atk, Def, SpAtk, SpDef, Speed;
}

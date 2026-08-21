using Godot;
using System;


[GlobalClass]
public partial class StatusCondition : Resource
{
    [Export] public StatusBase Base;
    [Export] public int MaxTime, Time;
}

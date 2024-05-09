using Godot;
using System;

[GlobalClass]
public partial class Moves : Resource
{
    [Export] public MoveBase Base{get; private set;}
}

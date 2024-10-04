using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Flags : Resource
{
    [Export] public Array<bool> ItemGiven;
    [Export] public Array<bool> PuzzleFlags;
}

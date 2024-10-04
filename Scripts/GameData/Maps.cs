using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public enum Areas{
    Water
}
[GlobalClass]
public partial class Maps : Resource
{
    [Export] public Array<PackedScene> maps;
}
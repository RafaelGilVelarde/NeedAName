using Godot;
using Godot.Collections;
using System;

public partial class WaterCurrents : Node
{
    [Export] public Array<TileMap> Currents;
    [Export] public int CurrentEventIndex;
}

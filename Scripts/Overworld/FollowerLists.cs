using Godot;
using Godot.Collections;
using System;

public partial class FollowerLists : Node
{
    [Export] public Array<int> ZIndexList;
	[Export] public Array<uint> CLayerList, CMaskList;
	[Export] public Array<Vector2> AxisList;
	[Export] public Array<Vector2> PositionList, AuxPositionList;
}

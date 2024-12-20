using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class TypedItemList : Resource
{
    [Export] public Array<Items> items;
}

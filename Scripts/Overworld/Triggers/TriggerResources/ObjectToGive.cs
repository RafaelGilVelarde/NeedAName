using Godot;
using System;

public enum ObjectType
{
    Move,
    Item
}
[GlobalClass]
public partial class ObjectToGive : Resource
{
    [Export] public Items Item;
    [Export] public Moves Move;
    [Export] public ObjectType objectType;
    [Export] public int index, PartyIndex;
    [Export] public string Timeline, KeyIndex;
    [Export] public Variant KeyValue;
}

using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Timelines : Resource
{
    [Export] public Array<string> DialogueTimelines;
    [Export] public Array<int> FlagIndexes;
    [Export] public Array<FlagType> flagTypes;
}

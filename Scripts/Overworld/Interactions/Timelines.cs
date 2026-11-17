using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Timelines : Resource
{
    [Export] public Array<string> DialogueTimelines, KeyIndexes;
    [Export] public Array<Variant> Variants;

    [Export] public Dictionary<string,Variant> TimelineDict;
}

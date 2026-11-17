using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class QuestEffectsList : Resource
{
    [Export] public Array<InteractEffectResource> Effects;
    [Export] public int QuestState;
}

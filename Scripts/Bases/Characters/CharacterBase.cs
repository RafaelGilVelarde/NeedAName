using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class CharacterBase : Resource
{
    [Export]public Stats BaseStats;
    [Export]public string Name;
    [Export]public PackedScene OverworldAnimator, BattleAnimator;
    [Export] public Color TextEffectColor;
}

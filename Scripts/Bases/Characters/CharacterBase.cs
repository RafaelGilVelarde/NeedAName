using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CharacterBase : Resource
{
    [Export] public Stats BaseStats;
    [Export] public string Name;
    [Export] public PackedScene OverworldAnimator, BattleAnimator;
    [Export] public Color TextEffectColor;
    [Export] public Texture Icon;
    [Export] public Vector2 OverworldOffset, BattleOffset, Floor;
    [Export] public Array<AudioStream> MoveSFX, HitSFX;
}

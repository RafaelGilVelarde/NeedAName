using Godot;
using GodotPlugins.Game;
using System;
using System.ComponentModel;

public partial class ProjectilePushEffect : ProjectileHit
{
    [Export] float Force, Decay;
    public override void HitEffect(Node2D HitTarget)
    {
        base.HitEffect(HitTarget);
        OverworldController HitCharacter = ((OverworldHurtbox)HitTarget).GetParent<OverworldController>();
        HitCharacter.ApplyForce(HitCharacter.GlobalPosition-Main.GlobalPosition,Force,Decay);     

    }
}

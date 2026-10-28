using Godot;
using System;

public partial class ProjectileHit : Node
{
    [Export] protected Node2D Main;
    public virtual void HitEffect(Node2D HitTarget = null)
    {
        /*MainProjectile.Acting = false;
        MainProjectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Hit",true);
		MainProjectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Start",false);*/
    }
}

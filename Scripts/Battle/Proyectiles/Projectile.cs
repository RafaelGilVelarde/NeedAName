using Godot;
using System;

public partial class Projectile : Node2D
{
        [Export] protected bool Overworld;
        [Export] public bool Ended, CanStart=true, Acting = true;
        protected double Time;

        public void HitWrapper(Node2D Aux)
    {
        Hit();
    }
        public virtual void Hit(){
        Acting = false;
        GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Hit",true);
		GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Start",false);
    }

}

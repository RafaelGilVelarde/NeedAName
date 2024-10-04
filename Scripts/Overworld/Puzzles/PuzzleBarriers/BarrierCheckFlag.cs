using Godot;
using System;

public partial class BarrierCheckFlag : PuzzleCheck
{
    [Export] AnimationPlayer Animator;
    [Export] CollisionShape2D Shape;
    [Export] int FlagIndex;
    [Export] string DisableAnimation;
    public override bool Check(){
        return GameManager.Instance.Data.Flags.PuzzleFlags[FlagIndex];
    }

    public override void ActivateEffect(){
        Shape.Disabled = true;
        Animator?.Play(DisableAnimation);
    }
}
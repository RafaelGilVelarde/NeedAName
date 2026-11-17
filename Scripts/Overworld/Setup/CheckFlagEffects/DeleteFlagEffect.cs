using Godot;
using System;

[GlobalClass]
public partial class DeleteFlagEffect : CheckFlagEffects
{
    public override void Effect(CheckFlagNode Check)
    {
        base.Effect(Check);
        Check.MainNode.QueueFree();
        Check.QueueFree();
    }
}

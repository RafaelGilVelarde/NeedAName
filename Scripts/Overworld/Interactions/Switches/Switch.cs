using Godot;
using System;

public partial class Switch : Interact
{
    [Export] int FlagIndex;
    [Export] PuzzleCheck Check;
    public override void _Ready()
    {
        base._Ready();
    }

    public override void interact(OverworldController Player)
    {
        GameManager.Instance.Data.Flags.PuzzleFlags[FlagIndex] = true;
        Check.ActivateEffect();
    }
}


using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class EnemyOrderPuzzle: PuzzleCheck
{
    [Export] Array<int> EnemiesBeaten = new Array<int>();
    [Export] int NextEnemy, TotalEnemies, FlagBoolIndex;

    public override void _Ready()
    {
        /*instance = this;
        GameManager.Instance.Data.Flags._PuzzleFlagsIntChanged+=EnemyDefeated;*/
    }
    public override void CheckInt(int Index, bool Changed)
    {
        Flags flags = GameManager.Instance.Data.Flags;
        if(Index == FlagIndex){
            if(flags.PuzzleIntFlags[Index] == TotalEnemies && Changed){
                ActivateEffect();
            }
        }
    }

    public override void ActivateEffect()
    {
        GameManager.Instance.Data.Flags.ChangeBoolFlag(FlagBoolIndex,true,FlagType.Puzzle);
        DialogicCSharp Dialog=DialogicCSharp.instance;
        Dialog.StartDialogue("PuzzleComplete",true,false);
    }



}
using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class EnemyOrderPuzzle: PuzzleCheck
{
    [Export] Array<int> EnemiesBeaten = new Array<int>();
    [Export] int NextEnemy, TotalEnemies;
    [Export] Variant ChangeValue;
    [Export] string ChangeKeyIndex;

    public override void _Ready()
    {
        Flags flags = GameManager.Instance.Data.Flags;
        if((int)flags.Dict[KeyIndex] != TotalEnemies)
        {
            flags.ChangeDictFlag(KeyIndex,0);
        }
        /*instance = this;
        GameManager.Instance.Data.Flags._PuzzleFlagsIntChanged+=EnemyDefeated;*/
    }
    public override void CheckDict(string Key, bool Changed)
    {
        Flags flags = GameManager.Instance.Data.Flags;
        if(Key == KeyIndex){
            if((int)flags.Dict[Key] == TotalEnemies && Changed){
                ActivateEffect();
            }
        }
    }

    public override void ActivateEffect()
    {
        GameManager.Instance.Data.Flags.ChangeDictFlag(ChangeKeyIndex,ChangeValue);
        DialogicCSharp Dialog=DialogicCSharp.instance;
        Dialog.StartDialogue("PuzzleComplete",true,false);
    }



}
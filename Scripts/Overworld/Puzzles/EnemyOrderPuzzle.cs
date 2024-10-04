using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class EnemyOrderPuzzle: Node
{
    public static EnemyOrderPuzzle instance;
    [Export] Array<int> EnemiesBeaten = new Array<int>();
    [Export] int NextEnemy, TotalEnemies, FlagIndex;

    public override void _Ready()
    {
        instance = this;
    }
    public void EnemyDefeated(int EnemyID){
        if(EnemyID == NextEnemy){
            EnemiesBeaten.Add(EnemyID);
            NextEnemy++;
            if(EnemiesBeaten.Count == TotalEnemies){
                GameManager.Instance.Data.Flags.PuzzleFlags[FlagIndex]=true;
            }
        }
        else{
            EnemiesBeaten.Clear();
            NextEnemy= 0;
        }
    }
    public override void _ExitTree()
    {
        instance = null;
    }


}
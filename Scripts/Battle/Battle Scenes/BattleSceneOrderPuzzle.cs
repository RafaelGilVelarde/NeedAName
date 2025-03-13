using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
[GlobalClass]
public partial class BattleSceneOrderPuzzle : BattleScene
{
    [Export] int Order;
    [Export] int FlagIndex;

    public override void ReturnToOverworld(){
        
        base.ReturnToOverworld();
        BattleManager Battle=BattleManager.instance;
        BattleState State=Battle.State;
        switch (State){
            case BattleState.Win:
                Flags Aux = GameManager.Instance.Data.Flags;
                if(Order == Aux.PuzzleIntFlags[FlagIndex]){
                    Aux.ChangeIntFlag(FlagIndex,Order+1,FlagType.Puzzle);
                }
                else{
                    Aux.ChangeIntFlag(FlagIndex,0,FlagType.Puzzle);
                }
                //EnemyOrderPuzzle.instance.EnemyDefeated(Order);
            break;
        }
    }
   

}

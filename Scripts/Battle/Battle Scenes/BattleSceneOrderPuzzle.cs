using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
[GlobalClass]
public partial class BattleSceneOrderPuzzle : BattleScene
{
    [Export] int Order;

    public override void BattleEnd(){
        
        BattleManager Battle=BattleManager.instance;
        BattleState State=Battle.State;
        switch (State){
            case BattleState.Win:
                EnemyOrderPuzzle.instance.EnemyDefeated(Order);
            break;
        }
        base.BattleEnd();
    }
   

}

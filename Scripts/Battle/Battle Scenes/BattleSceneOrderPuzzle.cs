using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
[GlobalClass]
public partial class BattleSceneOrderPuzzle : BattleScene
{
    [Export] int Order;
    [Export] string KeyIndex;

    public override void ReturnToOverworld(){
        
        base.ReturnToOverworld();
        BattleManager Battle=BattleManager.instance;
        BattleState State=Battle.State;
        switch (State){
            case BattleState.Win:
                Flags Aux = GameManager.Instance.Data.Flags;
                if((int)Aux.Dict[KeyIndex]==Order){
                    Aux.ChangeDictFlag(KeyIndex,Order+1);
                }
                else{
                    Aux.ChangeDictFlag(KeyIndex,0);
                }
                //EnemyOrderPuzzle.instance.EnemyDefeated(Order);
            break;
        }
    }
   

}

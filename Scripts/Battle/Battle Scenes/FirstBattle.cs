using Godot;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class FirstBattle : BattleScene
{
    [Export] string Timeline = "FirstBattle";
    public override void ReturnToOverworld()
    {
        base.ReturnToOverworld();
        if(!GameManager.Instance.Data.Flags.EventFlags[1]){
            BattleManager Battle=BattleManager.instance;
            BattleState State=Battle.State;
            switch (State){
                case BattleState.Win:
                    GameManager.Instance.Data.Flags.EventFlags[1] = true;
                    DialogicCSharp.instance.StartDialogue(Timeline,true,false);
                break;
            }
        }
    }
}

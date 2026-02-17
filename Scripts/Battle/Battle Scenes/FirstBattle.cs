using Godot;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class FirstBattle : BattleScene
{
    [Export] string Timeline = "FirstBattle";
    public override void StartTurnEffect()
    {
        base.StartTurnEffect();
        Debug.WriteLine("Flag: "+GameManager.Instance.Data.Flags.EventFlags[1]);
        Debug.WriteLine("TurnCount: "+Battle.TurnCount);
        if(Battle.TurnCount == 1 && !GameManager.Instance.Data.Flags.EventFlags[1]){
            Debug.WriteLine("Dialogue");
            Battle.CanStartTurn=false;
            GameManager.Instance.Data.Flags.EventFlags[1] = true;
            StartDialogue(Timeline,true,false,TimelineType.StartTurn);
        }
    }
    /*public override void ReturnToOverworld()
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
    }*/
}

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
        if(Battle.TurnCount == 1 && !(bool)GameManager.Instance.Data.Flags.Dict["BattleTutorial"]){
            Battle.CanStartTurn=false;
            GameManager.Instance.Data.Flags.Dict["BattleTutorial"] = true;
            StartDialogue(Timeline,true,false,TimelineType.StartTurn);
        }
    }

}

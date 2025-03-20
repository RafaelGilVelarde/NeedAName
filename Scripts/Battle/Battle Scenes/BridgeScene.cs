using Godot;
using Godot.Collections;
using System;
[GlobalClass]

public partial class BridgeScene : BattleScene
{
        [Export] Array<string> Timeline;
    public override void StartTurnEffect(){
        if(Battle.TurnCount == 1){
                Battle.CanStartTurn=false;
                DialogueFlag=true;
                StartDialogue(Timeline[Battle.CurrentTurn%Timeline.Count],false, false, TimelineType.StartTurn);
        }
        /*if(first==true){
            Stats stats=BattleManager.instance.EnemyParty[0].Character.stats;
            if(stats.MaxHP>stats.HP &&!DialogueFlag){
                Battle.CanStartTurn=false;
                Dialog.DialogicRoot.Connect("signal_event",endDialogue);
                DialogueFlag=true;
                Dialog.StartDialogue("BattleTest",false);
            }
        }*/
    }
}

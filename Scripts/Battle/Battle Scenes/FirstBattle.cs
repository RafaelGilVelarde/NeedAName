using Godot;
using System;

[GlobalClass]
public partial class FirstBattle : BattleScene
{
    [Export] string Timeline = "NPC1";
    public override void ReturnToOverworld()
    {
        base.ReturnToOverworld();
        if(!GameManager.Instance.Data.Flags.EventFlags[1]){
            GameManager.Instance.Data.Flags.EventFlags[1] = true;
            BattleManager Battle=BattleManager.instance;
            BattleState State=Battle.State;
            switch (State){
                case BattleState.Win:
                    DialogicCSharp.instance.StartDialogue(Timeline,true,false);
                break;
            }
        }
    }
}

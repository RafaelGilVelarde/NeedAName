using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

[GlobalClass]
public partial class ChangeQuestFlag : BattleScene
{
    [Export] int QuestIndex;
    [Export] int NewQuestState;
    [Export] string QuestKey;
    [Export] bool CompleteQuest;
    public override void ReturnToOverworld(){
        
        base.ReturnToOverworld();
        BattleManager Battle=BattleManager.instance;
        BattleState State=Battle.State;
        switch (State){
            case BattleState.Win:
                Debug.WriteLine("Changing Flag");
                Flags Aux = GameManager.Instance.Data.Flags;
                //Aux.ChangeDictFlag(DictKey,Value);
                ((Quest)Aux.Dict[QuestKey]).UpdateQuestState(NewQuestState,CompleteQuest);
            break;
        }
    }
}

using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ItemCheckQuestBase : QuestBase
{
    [Export] Array<Array<Items>> ItemChecks;
    [Export] Array<string> Timelines;
    public override void StartQuest(Quest quest)
    {
        base.StartQuest(quest);



    }
    public override void EndQuest(Quest quest)
    {
        base.EndQuest(quest);
    }
    public override void ConnectSignals(Quest quest)
    {
        base.ConnectSignals(quest);
        Array<TypedItemList> ItemLists = GameManager.Instance.Data.items;
        for(int i = 0; i < ItemLists.Count; i++)
        {
            ItemLists[i]._ItemAdded+=quest.TriggerCheck;
        } 
    }
    public override void DisconnectSignals(Quest quest)
    {
        base.DisconnectSignals(quest);
        Array<TypedItemList> ItemLists = GameManager.Instance.Data.items;
        for(int i = 0; i < ItemLists.Count; i++)
        {
            ItemLists[i]._ItemAdded-=quest.TriggerCheck;
        } 
    }
    public override void Check(Quest quest)
    {
        base.Check(quest);
        Array<TypedItemList> ItemLists = GameManager.Instance.Data.items;
        Array<int> Indexes = new Array<int>();
        for(int i = 0; i < ItemChecks.Count; i++)
        {
            int Aux = 0;
            for(int j = 0; j < ItemChecks[i].Count; j++)
            {
                for(int k = 0; k < ItemLists[(int)ItemChecks[i][j].Base.type].items.Count; k++)
                {
                    Items AuxItem = ItemLists[(int)ItemChecks[i][j].Base.type].items[k];
                    if( AuxItem.Base == ItemChecks[i][j].Base && AuxItem.Amount<=ItemChecks[i][j].Amount)
                    {
                        Aux++;
                    }
                }
            }
            if(Aux == ItemChecks.Count)
            {
                Indexes.Add(Aux);
            }
        }
        for(int i = 0; i < Indexes.Count; i++)
        {
            DialogicCSharp.instance.StartDialogue(Timelines[Indexes[i]],true,false);
        }
    }

}

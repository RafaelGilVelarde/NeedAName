using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ChangeQuestInteractEffectResource : InteractEffectResource
{
    [Export] Array<string> QuestKey;
    [Export] Array<int> QuestInt, QuestState;

    [Export] Array<bool> SetComplete;

    public override void ConnectCall()
    {
        base.ConnectCall();
        StartInteract = new Callable(this,MethodName.change);

    }
    void change(string argument)
    {
        string[] Split = argument.Split("_");
        Dictionary<string,Variant> Dict = GameManager.Instance.Data.Flags.Dict;
        if(Split[0] == "ChangeQuest")
        {
            if (Split.Length > 1)
            {
                int Index = Split[1].ToInt();
                ((Array<Quest>)Dict[QuestKey[Index]])[QuestInt[Index]].UpdateQuestState(QuestState[Index],SetComplete[Index]);
            }
            else
            {
                ((Array<Quest>)Dict[QuestKey[0]])[QuestInt[0]].UpdateQuestState(QuestState[0],SetComplete[0]);
            }
        }
    }

}

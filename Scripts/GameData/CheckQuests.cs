using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class CheckQuests : Resource
{
    [Export] string QuestKey;
    [Export] int QuestIndex, QuestState;
    [Export] bool CheckDate;

    public bool Check()
    {
        bool Aux = false;
        Quest CurrentQuest = ((Array<Quest>)GameManager.Instance.Data.Flags.Dict[QuestKey])[QuestIndex];
        if (CurrentQuest.CurrentQuestState == QuestState)
        {
            if (CheckDate)
            {
                if(CurrentQuest.Base.DayAvailability.Contains((int)GameManager.Instance.Data.Flags.Dict["Date"]));
            }
            else
            {
                Aux = true;
            }
        }
        return Aux;
    }

}

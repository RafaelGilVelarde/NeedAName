using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class GiveSidequestInteractEffectResource : InteractEffectResource
{
    [Export] Array<Quest> quest;
    [Export] Array<string> DictKey;

    public override void Ready()
    {
        base.Ready();
    }
    public override void ConnectCall()
    {
        base.ConnectCall();        
        StartInteract = new Callable(this,MethodName.GiveQuest);
    }
    void GiveQuest(string argument)
    {
        string[] Split = argument.Split("_");
        if(Split[0] == "GiveQuest")
        {
            if (Split.Length > 1)
            {
                int Index = Split[1].ToInt();
                GameManager.Instance.Data.Flags.ChangeDictArrayFlag(DictKey[Index],new Array<int> {quest[Index].Base.GameID}, new Array<Variant> {quest});                
                ((Array<Quest>)GameManager.Instance.Data.Flags.Dict[DictKey[Index]])[quest[Index].Base.GameID].StartQuest();
            }
            else
            {
                GameManager.Instance.Data.Flags.ChangeDictArrayFlag(DictKey[0],new Array<int> {quest[0].Base.GameID}, new Array<Variant> {quest});                
                ((Array<Quest>)GameManager.Instance.Data.Flags.Dict[DictKey[0]])[quest[0].Base.GameID].StartQuest();
            }
        }
    }
}

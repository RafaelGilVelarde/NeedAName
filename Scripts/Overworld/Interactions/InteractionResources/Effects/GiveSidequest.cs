using Godot;
using Godot.Collections;
using System;

public partial class GiveSidequest : InteractEffect
{
    [Export] Quest quest;
    [Export] string DictKey;
    public void GiveQuest()
    {
        GameManager.Instance.Data.Flags.ChangeDictArrayFlag(DictKey,new Array<int> {quest.Base.GameID}, new Array<Variant> {quest});
    }
}

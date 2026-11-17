using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class QuestBase : Resource
{
    [Export] public int GameID, Days;
    [Export] public string Title, Description, DictKey;
    [Export] public Array<string> Logs;
    [Export] public Array<int> LogStateCheck, DayAvailability;
    [Export] public Array<QuestEffectsList> Effects;

    public virtual void StartQuest(Quest quest)
    {
        ConnectSignals(quest);
    }
    public virtual void EndQuest(Quest quest)
    {
        DisconnectSignals(quest);
    }
    public virtual void ConnectSignals(Quest quest)
    {
        
    }
    public virtual void DisconnectSignals(Quest quest)
    {
        
    }

    public virtual void Check(Quest quest)
    {
        
    }
} 

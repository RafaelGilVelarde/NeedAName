using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Quest : Resource
{
    [Export] public QuestBase Base {get; private set;}
    [Export] public Array<int> CompletedQuestStates;
    [Export] public int CurrentQuestState;
    [Export] public bool Completed, TurnedIn, InstantlyEmit;
    [Export] public string Timeline;

    Callable DisconnectDialogue;

    [Signal] public delegate void _TurnInQuestEndedEventHandler();

        public void TurnInQuest()
        {
            TurnedIn = true;
            if (Timeline != null)
              {
                DisconnectDialogue = new Callable(this,MethodName.DisconnectTimeline);
                for(int i = 0; i < Base.Effects?[CurrentQuestState].Effects.Count; i++)
                {
                    Base.Effects[CurrentQuestState].Effects[i].ConnectCall();
                    DialogicCSharp.instance.DialogicRoot.Connect("signal_event",Base.Effects[CurrentQuestState].Effects[i].StartInteract);
                    DialogicCSharp.instance.DialogicRoot.Connect("timeline_ended",Base.Effects[CurrentQuestState].Effects[i].EndInteract);
                }
                DialogicCSharp.instance.DialogicRoot.Connect("timeline_ended",DisconnectDialogue);
              }
            Base.EndQuest(this);
        }

        public void DisconnectTimeline()
        {
            if (Timeline != null)
              {
                DisconnectDialogue = new Callable(this,MethodName.DisconnectTimeline);
                for(int i = 0; i < Base.Effects?[CurrentQuestState].Effects.Count; i++)
                {
                    DialogicCSharp.instance.DialogicRoot.Disconnect("signal_event",Base.Effects[CurrentQuestState].Effects[i].StartInteract);
                    DialogicCSharp.instance.DialogicRoot.Disconnect("timeline_ended",Base.Effects[CurrentQuestState].Effects[i].EndInteract);
                }
                DialogicCSharp.instance.DialogicRoot.Disconnect("timeline_ended",DisconnectDialogue);
              }
            if (InstantlyEmit)
            {
                EmitSignal("_TurnInQuestended");
            }
        }

    public void UpdateQuestState(int Update, bool Completed)
    {
        Quest Aux = (Quest)Duplicate();
        Aux.CurrentQuestState = Update;
        Aux.Completed = Completed;
        if (!Aux.CompletedQuestStates.Contains(Update))
        {
            Aux.CompletedQuestStates.Add(Update);
        }
        GameManager.Instance.Data.Flags.ChangeDictArrayFlag(Base.DictKey,new Array<int>{Base.GameID},new Array<Variant>{Aux});
    }
    public void StartQuest()
    {
        Base.StartQuest(this);   
    }

    public void TriggerCheck()
    {
        Base.Check(this);
    }
    }

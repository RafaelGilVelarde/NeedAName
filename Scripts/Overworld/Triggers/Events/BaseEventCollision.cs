using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;

public partial class BaseEventCollision : Area2D
{

    [Export] Dictionary<string, Variant> CheckDict, ChangeDict;
    [Export] int CurrentTimeline, TimelineIndex;
    [Export] Array<Timelines> Timelines;

    [Export] Array<CheckValueTimeline> CheckValues;


    [Export] protected Array<float> TweenTimes;
    [Export] protected Array<DialogueVariables> Variables;
    [Export] protected Array<InteractEffect> Effects;
    [Export] protected Array<InteractEffectResource> EffectResources;

    [Export] Array<CheckQuests> checkQuests = new Array<CheckQuests>();

    Callable disable;

    public override void _EnterTree()
    {
        BodyEntered += OnCollisionEntered;
    }
    public override void _Ready()
    {
        SceneTreeTimer Timer = GetTree().CreateTimer(0.5, true, true);
        disable = new Callable(this, MethodName.Disable);
        for(int i = 0; i < EffectResources.Count; i++)
        {
            EffectResources[i].BaseNode = this;
            EffectResources[i].Ready();
        }


    }
    protected virtual void OnCollisionEntered(Node2D body)
    {
        int auxcheck = 0;
        DataManager Data = GameManager.Instance.Data;
        for(int i = 0; i < CheckDict.Keys.Count; i++)
        {
            string AuxKey = CheckDict.Keys.ElementAt(i);
            if (Data.Flags.Dict[AuxKey].Equals(CheckDict[AuxKey]))
            {
                auxcheck++;
            }
        }

        for(int i = 0; i < checkQuests.Count; i++)
        {
            if (checkQuests[i].Check())
            {
                auxcheck++;
            }
        }


        int AuxCheckTarget = CheckDict.Keys.Count+checkQuests.Count;
        if (body.IsInGroup("PlayerOverworldController"))
        {
            if (auxcheck == AuxCheckTarget)
            {
                DialogicCSharp Dialogic = DialogicCSharp.instance;
                
                Dialogic.DialogicRoot.Connect("timeline_ended", disable);

                for (int i = 0; i < CheckValues.Count; i++)
                {
                    CurrentTimeline = CheckValues[i].GetTimeline(CurrentTimeline);
                }


                    for(int i = 0; i < ChangeDict.Keys.Count; i++)
                    {
                        string AuxKey = ChangeDict.Keys.ElementAt(i);
                        Data.Flags.ChangeDictFlag(AuxKey,ChangeDict[AuxKey]);
                    }               

                for(int i = 0; i < EffectResources?.Count; i++)
                {
                    EffectResources[i].ConnectCall();
                    Dialogic.DialogicRoot.Connect("signal_event",EffectResources[i].StartInteract);
                    Dialogic.DialogicRoot.Connect("timeline_ended",EffectResources[i].EndInteract);
                }        

                if (Variables != null)
                {
                    for (int i = 0; i < Variables.Count; i++)
                    {
                        Variables[i].SetVariable();
                    }            
                }
                Dialogic.StartDialogue(Timelines[CurrentTimeline].DialogueTimelines[TimelineIndex], true, false);
            }
        }
    }
 
    protected virtual void Disable()
    {
        DialogicCSharp Dialogic = DialogicCSharp.instance;

        for(int i = 0; i < EffectResources?.Count; i++)
        {
            Dialogic.DialogicRoot.Disconnect("signal_event",EffectResources[i].StartInteract);
            Dialogic.DialogicRoot.Disconnect("timeline_ended",EffectResources[i].EndInteract);
        }

        Dialogic.DialogicRoot.Disconnect("timeline_ended", disable);
    }
}

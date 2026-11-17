using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;

public partial class InteractText : Interact
{
    [Export] protected Array<Timelines> TimelineGroup;
    [Export] protected int TimelineGroupIndex, TimelineIndex;
    [Export] protected string Timeline = "Test";
    [Export] bool PauseWhenDialogue;
  
    [Export] Array<DialogueVariables> Variables;
    [Export] Array<InteractEffect> Effects = new Array<InteractEffect>();
    [Export] Array<InteractEffectResource> EffectResources = new Array<InteractEffectResource>();


    Callable disable, CheckTimeline;
    public override void _Ready()
    {
        base._Ready();
        disable = new Callable(this, MethodName.Disable);
        CheckTimeline = new Callable(this,MethodName.CheckIfCurrentTimeline);

        for(int i = 0; i < EffectResources.Count; i++)
        {
            Debug.WriteLine("Ready effect: "+i);
            EffectResources[i].BaseNode = this;
            EffectResources[i].Ready();
        }

    }


    public override void interact(OverworldController Player)
    {
        DialogicCSharp.instance.DialogicRoot.Connect("timeline_started",CheckTimeline);
        if (SpokenTo && TimelineIndex < TimelineGroup[TimelineGroupIndex].DialogueTimelines.Count - 1)
        {
            TimelineIndex++;
        }
        Enable(TimelineIndex);
        SpokenTo = true;

 
    }

    public void Enable(int id)
    {
        Timeline = TimelineGroup[TimelineGroupIndex].DialogueTimelines[id];
        Debug.WriteLine("Tiemline: " + Timeline + " id: " + id + " count: " + TimelineGroup[TimelineGroupIndex].DialogueTimelines.Count);
        DialogicCSharp DialogicInstance = DialogicCSharp.instance;
        //DialogicInstance.DialogicRoot.Connect("timeline_ended", disable);


        /*if (Variables != null)
        {
            for (int i = 0; i < Variables.Count; i++)
            {
                Variables[i].SetVariable();
            }            
        }*/
        DialogicInstance.StartDialogue(Timeline, PauseWhenDialogue, false);
    }
    public virtual void Disable()
    {
        SetFlags();
        Array<PlayerController> party = GameManager.Instance.Characters;
        for (int i = 0; i < party.Count; i++)
        {
            if (party[i].Leader)
            {
                party[i].InteractCollider.GetChild<CollisionShape2D>(0).Disabled = false;
            }
        }
        DialogicCSharp DialogicInstance = DialogicCSharp.instance;
        DialogicInstance.DialogicRoot.Disconnect("timeline_ended", disable);
        DialogicInstance.DialogicRoot.Disconnect("timeline_started", CheckTimeline);
        Debug.WriteLine("Effectresources: "+EffectResources?.Count);
        for(int i = 0; i < EffectResources?.Count; i++)
        {
            Debug.WriteLine("Disconnecting Effects");
            DialogicInstance.DialogicRoot.Disconnect("signal_event",EffectResources[i].StartInteract);
            DialogicInstance.DialogicRoot.Disconnect("timeline_ended",EffectResources[i].EndInteract);
        }

    }

    void SetFlags()
    {
        Timelines timeline = TimelineGroup[TimelineGroupIndex];
        Flags flags = GameManager.Instance.Data.Flags;
        if (timeline.TimelineDict != null)
        {
            for(int i = 0; i < timeline.TimelineDict.Keys.Count; i++)
            {
                string KeyAux = timeline.TimelineDict.Keys.ElementAt(i);
                flags.ChangeDictFlag(KeyAux,timeline.TimelineDict[KeyAux]);
            }            
        }
    }
    void CheckIfCurrentTimeline()
    {
        DialogicCSharp DialogicInstance = DialogicCSharp.instance;
        if(DialogicInstance.Timelines[0] == Timeline)
        {
            DialogicInstance.DialogicRoot.Connect("timeline_ended", disable);
            for(int i = 0; i < EffectResources?.Count; i++)
            {
                Debug.WriteLine("Connecting events");
                EffectResources[i].ConnectCall();
                DialogicInstance.DialogicRoot.Connect("signal_event",EffectResources[i].StartInteract);
                DialogicInstance.DialogicRoot.Connect("timeline_ended",EffectResources[i].EndInteract);
            }            
            if (Variables != null)
            {
                for (int i = 0; i < Variables.Count; i++)
                {
                    Variables[i].SetVariable();
                }            
            }
        }
    }
}

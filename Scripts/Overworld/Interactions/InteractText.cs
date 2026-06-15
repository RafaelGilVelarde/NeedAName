using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class InteractText : Interact
{
    [Export] protected Array<Timelines> TimelineGroup;
    [Export] protected int TimelineGroupIndex, TimelineIndex;
    [Export] protected string Timeline = "Test";
    [Export] bool PauseWhenDialogue, Turn;
  
    [Export] Array<DialogueVariables> Variables;
    [Export] Array<InteractEffect> Effects = new Array<InteractEffect>();


    Callable disable, beginCutscene, endCutscene;
    public override void _Ready()
    {
        base._Ready();
        disable = new Callable(this, MethodName.Disable);
        SceneTreeTimer Timer = GetTree().CreateTimer(0.5, true, true);
        Timer.Timeout += () =>
        {
            if (GameManager.Instance.CurrentScene.CutsceneAnimator != null && Cutscene)
            {
                CutsceneAnimator = GameManager.Instance.CurrentScene.CutsceneAnimator;
                beginCutscene = new Callable(this, MethodName.StartCutscene);
                endCutscene = new Callable(this, MethodName.EndCutscene);
            }
        };
    }


    public override void interact(OverworldController Player)
    {
        if (Turn)
        {
            FacingDirection = Player.FacingDirection * -1;
            if (FacingDirection.X != 0)
            {
                Flip();
            }
            AnimatorTree?.Set("parameters/Idle/blend_position", new Vector2(FacingDirection.X, -FacingDirection.Y));
        }
        for(int i = 0; i < Effects?.Count; i++)
        {
            Effects[i].ConnectCall();
            DialogicCSharp.instance.DialogicRoot.Connect("signal_event",Effects[i].StartInteract);
            DialogicCSharp.instance.DialogicRoot.Connect("timeline_ended",Effects[i].EndInteract);
        }
        if (SpokenTo && TimelineIndex < TimelineGroup[TimelineGroupIndex].DialogueTimelines.Count - 1)
        {
            TimelineIndex++;
        }
        SpokenTo = true;

        Enable(TimelineIndex);
 
    }

    public void Enable(int id)
    {
        Timeline = TimelineGroup[TimelineGroupIndex].DialogueTimelines[id];
        Debug.WriteLine("Tiemline: " + Timeline + " id: " + id + " count: " + TimelineGroup[TimelineGroupIndex].DialogueTimelines.Count);
        DialogicCSharp DialogicInstance = DialogicCSharp.instance;
        DialogicInstance.DialogicRoot.Connect("timeline_ended", disable);
        if (Cutscene)
        {
            DialogicInstance.DialogicRoot.Connect("signal_event",beginCutscene);
            DialogicInstance.DialogicRoot.Connect("signal_event",endCutscene);            
        }

        if (Variables != null)
        {
            for (int i = 0; i < Variables.Count; i++)
            {
                Variables[i].SetVariable();
            }            
        }
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
        for(int i = 0; i < Effects?.Count; i++)
        {
            DialogicInstance.DialogicRoot.Disconnect("signal_event",Effects[i].StartInteract);
            DialogicInstance.DialogicRoot.Disconnect("timeline_ended",Effects[i].EndInteract);
        }
        if (Cutscene)
        {
            DialogicInstance.DialogicRoot.Disconnect("signal_event",beginCutscene);
            DialogicInstance.DialogicRoot.Disconnect("signal_event",endCutscene);            
        }
    }
    protected virtual void Flip()
    {
        if (FacingDirection.X / Mathf.Abs(FacingDirection.X) > 0 != FacingRight)
        {
            Scale = new Vector2(Scale.X * -1, Scale.Y);
            FacingRight = !FacingRight;
        }
    }
    protected virtual void StartCutscene(string Cutscene)
    {
        if (CutsceneAnimator.CutsceneNames.Contains(Cutscene))
        {
            CutsceneAnimator.PlayAnimation(Cutscene);
        }
    }
    protected virtual void EndCutscene(string Argument)
    {
        if (Argument == "EndCutscene")
        {
            CutsceneAnimator.EndAnimation();            
        }
    }
    void SetFlags()
    {
        Timelines timeline = TimelineGroup[TimelineGroupIndex];
        Array<FlagType> flagTypes = timeline.flagTypes;
        Flags flags = GameManager.Instance.Data.Flags;
        if (flagTypes != null)
        {
            for (int i = 0; i < flagTypes.Count; i++)
            {
                switch (flagTypes[i])
                {
                    case FlagType.Puzzle:
                        flags.ChangeBoolFlag(timeline.FlagIndexes[i], true, FlagType.Puzzle);
                        break;
                    case FlagType.Event:
                        flags.ChangeBoolFlag(timeline.FlagIndexes[i], true, FlagType.Event);
                        break;
                    case FlagType.Item:
                        flags.ChangeBoolFlag(timeline.FlagIndexes[i], true, FlagType.Item);
                        break;
                    case FlagType.Dialogue:
                        flags.ChangeBoolFlag(timeline.FlagIndexes[i], true, FlagType.Dialogue);
                        break;
                }
            }            
        }
    }
}

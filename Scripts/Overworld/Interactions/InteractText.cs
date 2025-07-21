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
    [Export] protected bool SpokenTo, FacingRight = true, Cutscene;
    [Export] protected CutscenePlayer CutsceneAnimator;
    [Export] Array<DialogueVariables> Variables;


    Callable disable, beginCutscene, endCutscene;
    public override void _Ready()
    {
        base._Ready();
        disable = new Callable(this, MethodName.Disable);
        SceneTreeTimer Timer = GetTree().CreateTimer(0.5, true, true, true);
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
        if (SpokenTo && TimelineIndex < TimelineGroup[TimelineGroupIndex].DialogueTimelines.Count - 1)
        {
            TimelineIndex++;
        }
        SpokenTo = true;

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
                Debug.WriteLine("AAAA");
            }            
        }
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
}

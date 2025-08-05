using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class BaseEventCollision : Area2D
{
    [Export] int BoolIndex, CurrentTimeline, TimelineIndex;
    [Export] Array<Timelines> Timelines;
    [Export] protected bool Cutscene, ManualBoolUpdate;
    [Export] protected Array<Vector2> PushOffset;
    [Export] protected Array<float> PushTimes;


    [Export] Array<CheckValueTimeline> CheckValues;
    [Export] CheckValueTimeline CheckManualDisableValues;

    [Export] protected CutscenePlayer CutsceneAnimator;
    [Export] protected Array<float> TweenTimes;
    [Export] protected Array<DialogueVariables> Variables;
    Callable beginCutscene, endCutscene, disable, changeFlag, push;

    public override void _EnterTree()
    {
        BodyEntered += OnCollisionEntered;
    }
    public override void _Ready()
    {
        SceneTreeTimer Timer = GetTree().CreateTimer(0.5, true, true, true);
        disable = new Callable(this, MethodName.Disable);
        push = new Callable(this, MethodName.Push);
        changeFlag = new Callable(this, MethodName.ManualFlagUpdate);
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
    protected virtual void OnCollisionEntered(Node2D body)
    {
        if (body.IsInGroup("PlayerOverworldController"))
        {
            DataManager Data = GameManager.Instance.Data;
            if (!Data.Flags.EventFlags[BoolIndex])
            {
                DialogicCSharp Dialogic = DialogicCSharp.instance;
                
                Dialogic.DialogicRoot.Connect("timeline_ended", disable);

                for (int i = 0; i < CheckValues.Count; i++)
                {
                    CurrentTimeline = CheckValues[i].GetTimeline(CurrentTimeline);
                }


                if (!ManualBoolUpdate)
                {
                    Data.Flags.ChangeBoolFlag(BoolIndex, true, FlagType.Event);
                }
                else
                {
                    Dialogic.DialogicRoot.Connect("signal_event", changeFlag);
                }

                if (Cutscene)
                {
                    Dialogic.DialogicRoot.Connect("signal_event", beginCutscene);
                    Dialogic.DialogicRoot.Connect("signal_event", endCutscene);
                }
                Dialogic.DialogicRoot.Connect("signal_event", push);
                        
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
    protected virtual void StartCutscene(string Cutscene)
    {
        if (CutsceneAnimator.CutsceneNames.Contains(Cutscene))
        {
            CutsceneAnimator.PlayAnimation(Cutscene);
        }
    }
    protected virtual void Push(string argument)
    {
        string Aux = "";
        string Aux2 = "";
        if (argument.Contains("_"))
        {
            Aux = argument.Split("_")[0];
            Aux2 = argument.Split("_")[1];
        }
        if (Aux == "Push")
        {
            int Index = Aux2.ToInt();
            Tween tween = CreateTween();
            CharacterBody2D Chara = GameManager.Instance.Leader.Parent;
            tween.TweenProperty(Chara, "position", Chara.GlobalPosition + PushOffset[Index],PushTimes[Index]);
        }
    }
    protected virtual void ManualFlagUpdate(string argument)
    {
        string[] Aux = argument.Split("_");
        string Aux1 = "VariableType";
        int Aux2 = 0;//"Index";
        int Aux3 = 0;//"Value";
        int Aux4 = 0;//"FlagType";
        bool Aux3Bool = false;
        if (Aux.Length == 4)
        {
            Aux1 = Aux[0];
            Aux2 = Aux[1].ToInt();
            Aux3 = Aux[2].ToInt();
            Aux4 = Aux[3].ToInt();

            if (Aux3 == 0)
            {
                Aux3Bool = false;
            }
            else
            {
                Aux3Bool = true;
            }
        }
        Flags flags = GameManager.Instance.Data.Flags;
        Debug.WriteLine("Aux: " +Aux1);
        switch (Aux1)
        {
            case "Bool":
                Debug.WriteLine("Set: " + Aux3Bool);
                flags.ChangeBoolFlag(Aux2, Aux3Bool, (FlagType)Aux4);
                break;
            case "Int":
                flags.ChangeIntFlag(Aux2, Aux3, (FlagType)Aux4);
                break;
        }


    }
    protected virtual void EndCutscene(string Argument)
    {
        if (Argument == "EndCutscene")
        {
            CutsceneAnimator.EndAnimation();
        }
    }
    protected virtual void Disable()
    {
        DataManager Data = GameManager.Instance.Data;
        DialogicCSharp Dialogic = DialogicCSharp.instance;
        if (Cutscene)
        {
            Dialogic.DialogicRoot.Disconnect("signal_event", beginCutscene);
            Dialogic.DialogicRoot.Disconnect("signal_event", endCutscene);
        }
        if (ManualBoolUpdate)
        {
            Dialogic.DialogicRoot.Disconnect("signal_event", changeFlag);
            if (CheckManualDisableValues.GetTimeline(0) == 1)
            {
                SetDeferred("monitoring", false);
            }
        }
        if (Data.Flags.EventFlags[BoolIndex])
        {
            SetDeferred("monitoring", false);
        }
        Dialogic.DialogicRoot.Disconnect("signal_event", push);
        Dialogic.DialogicRoot.Disconnect("timeline_ended", disable);
    }
}

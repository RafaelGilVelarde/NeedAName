using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class BaseEventCollision : Area2D
{
    [Export] int BoolIndex, CurrentTimeline, TimelineIndex;
    [Export] Array<Timelines> Timelines;

    public override void _EnterTree()
    {
        BodyEntered+=OnCollisionEntered;
    }
    protected virtual void OnCollisionEntered(Node2D body)
    {
        if(body.IsInGroup("PlayerOverworldController")){
            DataManager Data = GameManager.Instance.Data;
            Debug.WriteLine(Data.Flags.EventFlags[BoolIndex]);
            if(!Data.Flags.EventFlags[BoolIndex]){
                 DialogicCSharp Dialogic = DialogicCSharp.instance;
                SetDeferred("monitoring",false);

                Dialogic.CallDeferred("StartDialogue",Timelines[CurrentTimeline].DialogueTimelines[TimelineIndex],true,false);
                Data.Flags.EventFlags[BoolIndex] = true;
            }
        }
    }
}

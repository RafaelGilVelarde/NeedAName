using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class InteractText : Interact
{
    [Export]protected Array<Timelines> TimelineGroup;
    [Export] protected int TimelineGroupIndex, TimelineIndex;
    [Export]protected string Timeline ="Test";
    [Export]bool PauseWhenDialogue, Turn;
    [Export] protected bool  SpokenTo, FacingRight = true;


    Callable disable;
    public override void _Ready()
    {
        base._Ready();
        disable=new Callable(this,MethodName.Disable);
    }


    public override void interact(OverworldController Player){
        if(Turn){
            FacingDirection = Player.FacingDirection*-1;
            if(FacingDirection.X!=0){
                Flip();
            }
            AnimatorTree?.Set("parameters/Idle/blend_position",new Vector2(FacingDirection.X,-FacingDirection.Y));
        }
        if(SpokenTo && TimelineIndex<TimelineGroup[TimelineGroupIndex].DialogueTimelines.Count-1){
            TimelineIndex++;
        }    
        SpokenTo=true;
        Enable(TimelineIndex);
    }

    public void Enable(int id){
        Timeline = TimelineGroup[TimelineGroupIndex].DialogueTimelines[id];
        Debug.WriteLine("Tiemline: "+Timeline+" id: "+id+" count: "+TimelineGroup[TimelineGroupIndex].DialogueTimelines.Count);
        DialogicCSharp DialogicInstance= DialogicCSharp.instance;
        DialogicInstance.DialogicRoot.Connect("timeline_ended",disable);
        DialogicInstance.StartDialogue(Timeline,PauseWhenDialogue,false);
    }
    public virtual void Disable(){
        Array<PlayerController> party=GameManager.Instance.Characters;
        for(int i=0;i<party.Count;i++){
            if(party[i].Leader){
                party[i].InteractCollider.GetChild<CollisionShape2D>(0).Disabled=false;
            }
        }
        DialogicCSharp DialogicInstance= DialogicCSharp.instance;
        DialogicInstance.DialogicRoot.Disconnect("timeline_ended",disable);
    }
    protected virtual void Flip(){
		if(FacingDirection.X/Mathf.Abs(FacingDirection.X)>0!=FacingRight){
			Scale=new Vector2(Scale.X*-1,Scale.Y);
            FacingRight=!FacingRight;
	    }
    }
}

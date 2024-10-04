using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class Interact : CollisionShape2D
{
    [Export]protected Array<string> Timelines;
    [Export] protected int TimelineIndex;
    [Export]protected string Timeline ="Test";
    [Export]bool PauseWhenDialogue, Turn;
    [Export] protected bool  SpokenTo;
    [Export] Vector2 FacingDirection;
    [Export] protected AnimationPlayer Animator;
    [Export] protected AnimationTree AnimatorTree;

    Callable disable;
    public override void _Ready()
    {
        if(GetChild(0)?.GetChildCount()>0){
            Node Aux=GetChild(0).GetChild(0);
            if(Aux.GetType()==typeof(AnimationPlayer)){
                Animator = (AnimationPlayer)Aux;

            }
        }
        if(Animator?.GetChildCount()>0){
            Node Aux=Animator.GetChild(0);
            if(Aux.GetType()==typeof(AnimationTree)){
                AnimatorTree = (AnimationTree)Aux;

            }
        }
    }


    public virtual void interact(OverworldController Player){
        if(Turn){
            FacingDirection = Player.FacingDirection*-1;
            if(FacingDirection.X!=0){
                Flip();
            }
            AnimatorTree?.Set("parameters/Idle/blend_position",new Vector2(FacingDirection.X,-FacingDirection.Y));
        }
        if(SpokenTo && TimelineIndex<Timelines.Count-1){
            TimelineIndex++;
        }    
        SpokenTo=true;
        Enable(TimelineIndex);
    }

    public void Enable(int id){
        Timeline = Timelines[id];

        disable=new Callable(this,MethodName.Disable);
        DialogicCSharp DialogicInstance= (DialogicCSharp)DialogicCSharp.instance;
        DialogicInstance.DialogicRoot.Connect("timeline_ended",disable);
        DialogicInstance.StartDialogue(Timeline,PauseWhenDialogue,false);
    }
    public virtual void Disable(){
        Array<OverworldController> party=GameManager.Instance.Characters;
        for(int i=0;i<party.Count;i++){
            if(party[i].Leader){
                party[i].InteractCollider.GetChild<CollisionShape2D>(0).Disabled=false;
            }
        }
        DialogicCSharp DialogicInstance= (DialogicCSharp)DialogicCSharp.instance;
        DialogicInstance.DialogicRoot.Disconnect("timeline_ended",disable);
    }
    void Flip(){
		if(FacingDirection.X/Mathf.Abs(FacingDirection.X)!=Scale.Y){
			Scale=new Vector2(Scale.X*-1,Scale.Y);
	    }
    }
}

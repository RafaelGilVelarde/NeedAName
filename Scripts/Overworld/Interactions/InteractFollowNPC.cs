using Godot;
using System;
using System.Diagnostics;

public partial class InteractFollowNPC : Interact
{
    Callable FollowNPC;
    [Export] PlayerController Follower;
    public override void _Ready()
    {
        base._Ready();
        FollowNPC = new Callable(this,MethodName.Follow);
        Animator = Follower.Animator;
        AnimatorTree = Follower.AnimatorTree;
        if(GameManager.Instance.Data.CurrentFollowers.Contains(Follower.BattleCharacter.Character)){
            GetParent().QueueFree();
        }
    }

    public override void interact(OverworldController Player)
    {
        Node DialogicRoot=DialogicCSharp.instance.DialogicRoot;
        DialogicRoot.Connect("signal_event",FollowNPC);
        base.interact(Player);
    }
    public override void Disable()
    {
        Node DialogicRoot=DialogicCSharp.instance.DialogicRoot;
        DialogicRoot.Disconnect("signal_event",FollowNPC);
        base.Disable();
    }
    void Follow(string argument){
        Follower.ProcessMode = ProcessModeEnum.Inherit;
        GameManager.Instance.AddFollowingCharacter(Follower,true);
    }
    protected override void Flip()
    {
		if(FacingDirection.X/Mathf.Abs(FacingDirection.X)!=GlobalScale.Y){
			GetParent<Node2D>().Scale=new Vector2(GlobalScale.X*-1,GlobalScale.Y);
	    }
    }
}

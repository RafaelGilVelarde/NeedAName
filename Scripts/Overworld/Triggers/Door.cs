using Godot;
using System;
using System.Diagnostics;

public partial class Door : Area2D
{
    [Export] int Scene;
    [Export] Areas Area;
    [Export] Vector2 LoadPosition;
    public override void _Ready()
    {
        Callable.From(ActorSetup).CallDeferred();	
    }
    private void OnCollisionEntered(Node2D body)
    {

        if(body.IsInGroup("PlayerOverworldController")){
            SetDeferred("monitoring",false);
            GameManager.Instance.CallDeferred("SwitchScene",Scene, (int)Area, LoadPosition);
        }
    }
    	private async void ActorSetup()
    {
        await ToSignal(GetTree().CreateTimer(0.1f,true),"timeout");
        BodyEntered+=OnCollisionEntered;        
    }
	
}

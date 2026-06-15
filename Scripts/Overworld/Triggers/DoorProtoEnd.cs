using Godot;
using System;

public partial class DoorProtoEnd : Area2D
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
            CallDeferred("SwitchScene");
        }
    }
    	private async void ActorSetup()
    {
        await ToSignal(GetTree().CreateTimer(0.1f,true,true),"timeout");
        BodyEntered+=OnCollisionEntered;        
    }
    void SwitchScene(){
        GetTree().ChangeSceneToPacked(GameManager.Instance.AreaMaps[(int)Area].maps[Scene]);
    }
	
}

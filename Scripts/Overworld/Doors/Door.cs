using Godot;
using System;

public partial class Door : Area2D
{
    [Export] PackedScene Scene;
    [Export] Vector2 LoadPosition;
    public override void _Ready()
    {
        BodyEntered+=OnCollisionEntered;
    }
    private void OnCollisionEntered(Node2D body)
    {
        if(body.IsInGroup("PlayerOverworldController")){
            GameManager.Instance.CallDeferred("SwitchScene",Scene,LoadPosition);
        }
    }
}

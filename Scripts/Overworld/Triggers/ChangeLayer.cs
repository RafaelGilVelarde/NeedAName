using Godot;
using System;

public partial class ChangeLayer : Area2D
{   
    [Export] int Z;
    public override void _Ready()
    {
        BodyEntered+=OnTriggerEnter;
    }    
    
    private void OnTriggerEnter(Node2D body) {
        if(body.IsInGroup("PlayerOverworldController")){
            CharacterBody2D Player = (CharacterBody2D)body;
            Player.CollisionMask = CollisionLayer;
            Player.ZIndex = Z;
        } 
    }
    
}

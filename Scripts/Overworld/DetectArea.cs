using Godot;
using System;
using System.Diagnostics;

public partial class DetectArea : Area2D
{
    public bool Entered;
    public override void _Ready()
    {
        BodyEntered+=OnCollisionEntered;
        BodyExited+=OnCollisionExited;
    }
    private void OnCollisionEntered(Node2D body)
    {
        if(body.IsInGroup("PlayerOverworldController")){
            Entered=true;
        }
    }
    private void OnCollisionExited(Node2D body)
    {
        if(body.IsInGroup("PlayerOverworldController")){
            Entered=false;
        }
    }

}

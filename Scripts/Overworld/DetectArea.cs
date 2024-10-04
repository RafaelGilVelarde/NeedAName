using Godot;
using System;
using System.Diagnostics;

public partial class DetectArea : Area2D
{
    public bool Entered = false;
    [Export] public Polygon2D Polygon;
    [Export] public CollisionPolygon2D Collision {get; private set;}
    public override void _EnterTree()
    {
        Polygon.Polygon = Collision.Polygon;
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

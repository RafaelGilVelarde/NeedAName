using Godot;
using Godot.Collections;
using System;

public partial class MovingObject : Node2D
{
    
    [Export] Array<TileCollisionChangeNode> CheckCollisionNode;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        for(int i = 0; i < CheckCollisionNode.Count; i++)
        {
            CheckCollisionNode[i].Disable();
        }
        for(int i = 0; i < CheckCollisionNode.Count; i++)
        {
            CheckCollisionNode[i].Enable();
        }
    }

}

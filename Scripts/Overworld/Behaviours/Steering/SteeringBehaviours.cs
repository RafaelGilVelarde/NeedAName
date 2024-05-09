using Godot;
using System;
using System.Collections.Generic;
public static class Directions
{
    public static List<Vector2> directions = new List<Vector2>
    {
        new Vector2(0,1),
        new Vector2(-1,1).Normalized(),
        new Vector2(-1,0),
        new Vector2(-1,-1).Normalized(),
        new Vector2(0,-1),
        new Vector2(1,-1).Normalized(),
        new Vector2(1,0),
        new Vector2(1,1).Normalized(),
    };
}
[GlobalClass]
public abstract partial class SteeringBehaviours : Resource
{
    public abstract (float[] danger, float[] interest) GetSteering(float[] danger, float[]interest,EnemyOverworldController enemy);
}

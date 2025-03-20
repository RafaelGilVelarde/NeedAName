using Godot;
using Godot.Collections;
using System;
public static class Directions
{
    public static Array<Vector2> directions = new Array<Vector2>
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

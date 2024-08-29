using Godot;
using System;
using System.Diagnostics;

public partial class BezierProyectile : Node2D
{
    [Export] public Vector2 PosStart, PosMiddle, PosEnd;
    [Export] public bool Ended, CanStart=true;
    double Time;
    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition=Bezier(Time);
        Time+=delta;
        if(Time>=1){
            Time=0;
        }
    }
    Vector2 Bezier(double Time){
        Vector2 Start=PosStart.Lerp(PosMiddle, (float)Time);
        Vector2 Middle=PosMiddle.Lerp(PosEnd,(float)Time);
        Vector2 Result=Start.Lerp(Middle,(float)Time);
        return Result;
    }

    public void Hit(){
        ProcessMode=ProcessModeEnum.Disabled;
        GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Hit",true);
		GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Start",false);
    }
    public void EndSplash(){
        if(Ended){
            QueueFree();
        }
        CanStart=true;
    }
    public void Start(){
            ProcessMode=ProcessModeEnum.Inherit;
            Time=0;
            CanStart=false;
    }
}

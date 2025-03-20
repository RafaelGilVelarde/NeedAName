using Godot;
using System;
using System.Diagnostics;

public partial class BezierProyectile : Node2D
{
    [Export] public Vector2 PosStart, PosMiddle, PosEnd;

    [Export] public BattleCharacter Target;
    [Export] public bool Ended, CanStart=true, Acting = true;
    [Export] protected float Speed = 1;
    protected double Time;
    public override void _PhysicsProcess(double delta)
    {
        if(Acting){
            GlobalPosition=Bezier(Time);
            Time+=delta*Speed;
            if(Time>=1){
                Time=0;
            }
        }
    }
    protected Vector2 Bezier(double Time){
        Vector2 Start=PosStart.Lerp(PosMiddle, (float)Time);
        Vector2 Middle=PosMiddle.Lerp(PosEnd,(float)Time);
        Vector2 Result=Start.Lerp(Middle,(float)Time);
        return Result;
    }

    public virtual void Hit(){
        Acting = false;
        GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Hit",true);
		GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Start",false);
    }
        void Disable(){
            ProcessMode=ProcessModeEnum.Disabled;
        }
    public virtual void EndSplash(){
        if(Ended){
            QueueFree();
        }
        CanStart=true;
    }
    public void Start(){
            ProcessMode=ProcessModeEnum.Inherit;
            Time=0;
            Acting = true;
            CanStart=false;
    }
}

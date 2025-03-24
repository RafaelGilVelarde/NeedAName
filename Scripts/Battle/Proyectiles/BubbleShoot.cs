using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Text;


public enum BubbleState{
    Normal,
    Shoot,

}
public partial class BubbleShoot : BezierProyectile
{
    [Export] BubbleState State;
    [Export] PackedScene MiniProyectile;
    [Export] Array<BezierProyectile> Proyectiles;
    [Export] int IntervalIndex, IntervalCount, ProyectileAmount, ProyMax;
    [Export] Array<float> Intervals,Limits;
    public override void _Ready()
    {
        base._Ready();
        IntervalCount = Intervals.Count;
    }
    public override void _PhysicsProcess(double delta)
    {
        if(Acting){

            switch (State)
            {
                
                case BubbleState.Normal:
                    GlobalPosition=Bezier(Time);
                    Time+=delta*Speed;
                    if(Time>=1){
                        Time=0;
                        State = BubbleState.Shoot;
                    }
                break;
                case BubbleState.Shoot:
                    Intervals[IntervalIndex%IntervalCount]+= (float)delta;
                    if(Intervals[IntervalIndex%IntervalCount]>=Limits[IntervalIndex%IntervalCount]){
                        Intervals[IntervalIndex%IntervalCount] = 0;
                        if(IntervalIndex%IntervalCount == 0){
                            IntervalIndex++;
                        }
                        else{
                            if(ProyectileAmount>ProyMax){
                                IntervalIndex++;
                                ProyectileAmount = 0;
                            }
                            else{
                                Shoot();
                            }
                        }
                    }
                break;
            }
        }
    }
    void Shoot(){
        ProyectileAmount++;
        RandomNumberGenerator RNG=new RandomNumberGenerator();
        float Middle = RNG.RandiRange(2,5);
        float Height = RNG.RandiRange(3,7);
        BezierProyectile proyectile = MiniProyectile.Instantiate<BezierProyectile>();
        Proyectiles.Add(proyectile);

        proyectile.Target = Target;
        proyectile.GlobalPosition = GlobalPosition;
        proyectile.PosStart = GlobalPosition;
        proyectile.PosEnd = Target.Hitbox.GetChild<CollisionShape2D>(0).GlobalPosition;
        float MiddleX =proyectile.PosStart.X+((proyectile.PosEnd.X-proyectile.PosStart.X)/Middle);
		proyectile.PosMiddle=new Vector2(MiddleX,GlobalPosition.Y-Height);

        GetTree().CurrentScene.AddChild(proyectile);

        Hitbox ThisHitbox = GetChild<Hitbox>(0);
        Hitbox ProyHitbox = proyectile.GetChild<Hitbox>(0);
        ProyHitbox.AddToGroup(GetChild<Hitbox>(0).GetGroups()[0]);
        ProyHitbox.Character = ThisHitbox.Character;
        ProyHitbox._Hit+=EmitHit;
        ProyHitbox._WP+=EmitWP;
        ProyHitbox._Block+=EmitBlock;
        void EmitHit(BattleCharacter Character){
            ThisHitbox.EmitSignal("_Hit",Character);
            proyectile.Ended = true;
            proyectile.Hit();
            Proyectiles.Remove(proyectile);
        }
        void EmitWP(BattleCharacter Character){
            ThisHitbox.EmitSignal("_WP",Character);
        }
        void EmitBlock(BattleCharacter Character){
            ThisHitbox.EmitSignal("_Block",Character);
        }
    }
    public override void Hit()
    {
        
        if(Ended){
            Acting = false;
            GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Hit",true);
            Debug.WriteLine("Shoot Ended: "+Ended);
            for(int i = 0;i<Proyectiles.Count;i++){
                Proyectiles[i].Ended = Ended;
                Proyectiles[i].Hit();
            }
        }
    }
    public override void EndSplash()
    {
        if(Ended){
            Debug.WriteLine("EndedProyeectile, queuefree");
            QueueFree();
        }
        Debug.WriteLine(IsQueuedForDeletion());
        
    }

}

using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class DummyBurrowMove : MoveBase
{
    [Export] BattleCharacter.BattleState State = BattleCharacter.BattleState.Dodging;
    [Export] float Speed, JumpTime, ExtraHeight;
    [Export] int Combo = 1;
	[Export] Vector2 offset;
	[Export] bool Movement = true;
	public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
	{
		base.Effect(Users,Targets);
		Tween tween = Users[0].CreateTween();
		BattleScene CurrentScene = BattleManager.instance.Scene;
        float VerticalDistance = 0, Aux1 = 0, Aux2 = 0;
		
		float Dir=(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X)/Mathf.Abs(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X);
		if(Dir==1||Dir==-1){
			Users[0].GetParent<Node2D>().Rotation=0;
			Users[0].GetParent<Node2D>().Scale=new Vector2(Dir,1);
			
			Targets[0].GetParent<Node2D>().Rotation=0;
			Targets[0].GetParent<Node2D>().Scale=new Vector2(-Dir,1);
		}		
		else{
			Dir=Users[0].GetParent<Node2D>().Scale.Y;
		}		
		Users[0].changeCombo(Combo);


	

		//Users[0].Hitbox._Hit+=onHit;
		//Users[0].Hitbox._Block+=blocked;

		Targets[0].Controllable=true;                



		//timer.TweenInterval(MoveTime);
		Vector2 TargetPosition = Users[0].GlobalPosition;
        
        Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/LoopStarted",true);
        Users[0].changeAction(BattleCharacter.ActionState.isAttacking);
        Targets[0].changeState(State);
        Targets[0].changeState(State);
		if(Movement){
			TargetPosition = Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-Users[0].BattleOffset;
			if (CurrentScene.Horizontal)
			{
				float FloorOffset = CurrentScene.PartyFloorY[Targets[0].PosIndex] - CurrentScene.EnemyFloorY[Users[0].PosIndex];
				TargetPosition = new Vector2(TargetPosition.X, Users[0].GlobalPosition.Y + FloorOffset);
			}
		}
		
        tween.TweenInterval(0.25);
		tween.TweenProperty(Users[0].GetParent(),"position",TargetPosition+offset*-Dir,1/Speed);
        tween.TweenInterval(0.25);
        tween.TweenCallback(Callable.From(JumpCalc));
		tween.TweenCallback(Callable.From(()=>Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/LoopEnded",true)));
		tween.TweenMethod(Callable.From((float t)=>Jump(t,(Node2D)Users[0].GetParent())), 0, JumpTime, MoveTime-0.6);


        tween.Finished+=End;
		tween.Finished+=tween.Kill;

       // Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",true);

        void JumpCalc()
        {
            VerticalDistance = Targets[0].GlobalPosition.Y-ExtraHeight-Users[0].GlobalPosition.Y;
            Aux1 = Mathf.Pow(VerticalDistance,2);
            Aux2 = 2*Aux1;
        }
        void Jump(float time,Node2D Object)
        {
            float Y = -Mathf.Pow(Aux2*time-Aux1,2)+VerticalDistance;
            if (time > JumpTime / 2)
            {
                Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/Falling",true);
            }
            Object.Position = new Vector2(Object.Position.X,Y);
        }

        void End()
        {
            Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/LoopEnded",false);
            Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/LoopStarted",false);
            Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/Falling",false);
        }    
	}
    
}

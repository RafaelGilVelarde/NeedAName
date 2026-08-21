using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class DummyStatusAttackMove : MoveBase
{
    /*[Export] StatusCondition Status;
    [Export] Array<StatusCondition> Alt;*/
    [Export] BattleCharacter.BattleState State = BattleCharacter.BattleState.Dodging;
    [Export] float MoveSpeed;
    [Export] int Combo = 1;
	[Export] Vector2 offset;
	[Export] bool Movement = true;

    [Export] StatChange Stats;

    /*[Export] int AtkTime, DefTime,SpAtkTime,SpDefTime, SpeedTime, StatusTime;
    [Export] float AtkMult,DefMult,SpAtkMult,SpDefMult,SpeedMult, HPIncrease, WPIncrease;
    [Export] protected bool Atk,Def,Spatk,Spdef,Speed, HP, WPUp, DelayUp, StatCondition, AltStat;*/
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
        base.Effect(Users, Targets);
        base.Effect(Users,Targets);
		Tween tween = Users[0].CreateTween();
		BattleScene CurrentScene = BattleManager.instance.Scene;
		
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


		Targets[0].Controllable=true;                



		Vector2 TargetPosition = Users[0].GlobalPosition;
		if(Movement){
			TargetPosition = Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-Users[0].BattleOffset;
			if (CurrentScene.Horizontal)
			{
				float FloorOffset = CurrentScene.PartyFloorY[Targets[0].PosIndex] - CurrentScene.EnemyFloorY[Users[0].PosIndex];
				TargetPosition = new Vector2(TargetPosition.X, Users[0].GlobalPosition.Y + FloorOffset);
			}
		}
		tween.TweenCallback(Callable.From(()=>Targets[0].changeState(State)));
		tween.TweenProperty(Users[0].GetParent(),"position",TargetPosition+offset*-Dir,1/MoveSpeed);
		tween.TweenCallback(Callable.From(()=>Users[0].changeState(BattleCharacter.BattleState.Attacking)));
		

		tween.TweenInterval(0.25);
		tween.TweenCallback(Callable.From(Auto));

		tween.Finished+=tween.Kill;

		//timer.Finished+=End;
		//timer.Finished+=timer.Kill;

		void Auto(){
			if(Users[0].actionState!=BattleCharacter.ActionState.isAttacking){
				Users[0].changeState(BattleCharacter.BattleState.Idle);
				Users[0].changeAction(BattleCharacter.ActionState.isAttacking);         
			}
		}
    }
    public override void Hit(BattleCharacter User, BattleCharacter Target)
    {
        SceneTreeTimer HitTimer = User.GetTree().CreateTimer(1/Engine.GetFramesPerSecond());
        HitTimer.Timeout+=Check;
        void Check()
        {
            if (!Target.BlockedEnemy)
            {
                DealDamage(User, Target, false);
            }
            else
            {
                DealDamage(User, Target, true);
            }
            Stats.RaiseStat(Target);
        }
    }

   
}

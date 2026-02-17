using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;
[GlobalClass]

public partial class DumyBeamMove : MoveBase
{
[Export] float Speed;
[Export] int Combo = 1,AtkMultiplier;
[Export] String Tag;
[Export] PackedScene LineShoot;
	[Export] Vector2 offset;
	public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
	{
		base.Effect(Users,Targets);
		Targets[0].Controllable=true;
		float Mult = Users[0].StatMultiplier[2];
		bool Success = false;


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
		
		Line2D Line=LineShoot.Instantiate<Line2D>();
		RayHitbox Ray= (RayHitbox)Line.GetChild(0);
		Ray.AddExceptions(new Array<CollisionObject2D>{Users[0].Hurtbox,Users[0].Hitbox,Users[0].Blockbox,Targets[0].Hitbox});
		if (Targets[0].WPbox != null)
		{
			Ray.AddException(Targets[0].WPbox);
		}
		Ray.AddToGroup(Tag);
		Users[0]._Shoot+=shoot;
		Ray._Hit+=onHit;
        Ray._Block+=blocked;
		Ray.Attacking=Users[0];
		Ray.TargetPosition=new Vector2(Dir*10000,0);

        void onHit(BattleCharacter Target){
			Hit(Users[0],Target);
		}		
		void blocked(BattleCharacter Target){
			Block(Target);
		}
		void ShootSetup(BattleCharacter User)
		{
			Debug.WriteLine($"Controlled: {User.Character.isControlledByPlayer}");
			if (User.Character.isControlledByPlayer)
			{
				SceneTreeTimer Timer = User.GetTree().CreateTimer(0.3, true, true, true);
				User.Character.ShowTextLabel($"{User.Character.Key}", User.Character.Base.TextEffectColor);
				User._DoAction += ManualShoot;
				User.changeState(BattleCharacter.BattleState.Attacking);
				Timer.Timeout += () =>
				{
					Debug.WriteLine("Shoot");
					User.changeState(BattleCharacter.BattleState.Idle);
					if (Success)
					{
						User.StatMultiplier[2] += AtkMultiplier;
						User.Character.ChangeWP(WP);
						User.changeCombo(2);
					}
					else
					{
						User._DoAction -= ManualShoot;
						User.changeAction(BattleCharacter.ActionState.isAttacking);
					}
					//shoot(User);
				};
			}
		}
		void ManualShoot()
		{
			Success = true;
			Users[0]._DoAction -= ManualShoot;
		}
		void shoot(BattleCharacter User)
		{
			Users[0]._Shoot-=shoot;
			//Users[0].SoundEffectController.PlaySFX(0,SFX);
			Line.GlobalPosition=User.ShootNode.GlobalPosition;
			Line.ClearPoints();
			Line.AddPoint(Vector2.Zero);
			Line.AddPoint(new Vector2(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X+Dir*20,0));
			User.GetTree().CurrentScene.AddChild(Line);			
		}

		//timer.TweenInterval(MoveTime);
				BattleScene CurrentScene = BattleManager.instance.Scene;

		Vector2 TargetPosition = Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-Users[0].BattleOffset;
		if (CurrentScene.Horizontal)
		{
			float FloorOffset = CurrentScene.EnemyFloorY[Targets[0].PosIndex] - CurrentScene.PartyFloorY[Users[0].PosIndex];
			TargetPosition = new Vector2(TargetPosition.X, Users[0].GlobalPosition.Y + FloorOffset);
		}

		Tween tween = Users[0].CreateTween();
		tween.TweenCallback(Callable.From(()=>Targets[0].changeState(BattleCharacter.BattleState.Defending)));
		tween.TweenProperty(Users[0].GetParent(),"position",TargetPosition+offset*-Dir,1/Speed);
		tween.TweenInterval(0.5);
		if(!Users[0].Character.isControlledByPlayer)
		{
			tween.TweenCallback(Callable.From(()=>Users[0].changeAction(BattleCharacter.ActionState.isAttacking)));			
		}


		tween.Finished += () =>
		{
			if (Users[0].Character.isControlledByPlayer)
			{
				Debug.WriteLine("EARSDgzvcx");
				ShootSetup(Users[0]);
			}
			tween.Kill();
		};
        MainTimer.Timeout += ()=>{Users[0].StatMultiplier[2] = Mult;};

		//timer.Finished+=End;
		//timer.Finished+=timer.Kill;

	}
}

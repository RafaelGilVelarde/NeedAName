using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;
[GlobalClass]

public partial class DumyBeamMove : MoveBase
{
[Export] float Speed;
[Export] int Combo = 1;
[Export] String Tag;
[Export] PackedScene LineShoot;
	[Export] Vector2 offset;
	public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
	{
		base.Effect(Users,Targets);
		Targets[0].Controllable=true;                


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

		Users[0].Combo=Combo;
		
		Line2D Line=LineShoot.Instantiate<Line2D>();
		RayHitbox Ray= (RayHitbox)Line.GetChild(0);
		Ray.AddExceptions(new Array<CollisionObject2D>{Users[0].Hurtbox,Users[0].Hitbox,Users[0].Blockbox,Targets[0].Hitbox});
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
		void shoot(BattleCharacter User){
			Line.GlobalPosition=User.ShootNode.GlobalPosition;
			Line.ClearPoints();
			Line.AddPoint(Vector2.Zero);
			Line.AddPoint(new Vector2(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X+Dir*10,0));
			User.GetTree().CurrentScene.AddChild(Line);
			User._Shoot-=shoot;
		}

		//timer.TweenInterval(MoveTime);

		Tween tween = Users[0].CreateTween();
		tween.TweenCallback(Callable.From(()=>Targets[0].changeState(BattleCharacter.BattleState.Defending)));
		tween.TweenProperty(Users[0].GetParent(),"position",Targets[0].GetParent<Node2D>().Position+offset*-Dir,1/Speed);
		tween.TweenCallback(Callable.From(()=>Users[0].changeAction(BattleCharacter.ActionState.isAttacking)));


		tween.Finished+=tween.Kill;

		//timer.Finished+=End;
		//timer.Finished+=timer.Kill;

	}
}

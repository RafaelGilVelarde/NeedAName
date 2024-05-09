using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
[GlobalClass]

public partial class DumyEnemyMove : MoveBase
{
[Export] float Speed;
	[Export] Vector2 offset;
	public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
	{
		base.Effect(Users,Targets);
		Tween tween = Users[0].CreateTween();
		float Dir=(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X)/Mathf.Abs(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X);


		int a=0;

	

		//Users[0].Hitbox._Hit+=onHit;
		//Users[0].Hitbox._Block+=blocked;

		Targets[0].Controllable=true;                


		if(Dir==1||Dir==-1){
			Users[0].GetParent<Node2D>().Rotation=0;
			Users[0].GetParent<Node2D>().Scale=new Vector2(Dir,1);
		}		
		else{
			Dir=Users[0].GetParent<Node2D>().Scale.Y;
		}
		//timer.TweenInterval(MoveTime);

		tween.TweenCallback(Callable.From(()=>Targets[0].changeState(BattleCharacter.BattleState.Dodging)));
		tween.TweenProperty(Users[0].GetParent(),"position",Targets[0].GetParent<Node2D>().Position+offset*-Dir,1/Speed);
		tween.TweenCallback(Callable.From(()=>Users[0].changeState(BattleCharacter.BattleState.Attacking)));
		

		tween.TweenInterval(0.25);
		tween.TweenCallback(Callable.From(Auto));

		tween.Finished+=tween.Kill;

		//timer.Finished+=End;
		//timer.Finished+=timer.Kill;

		void Auto(){
			Debug.WriteLine("EnemyMove");
			Debug.WriteLine(a);
			Debug.WriteLine(Users[0].actionState);
			if(Users[0].actionState!=BattleCharacter.ActionState.isAttacking&&a==0){
				Users[0].changeState(BattleCharacter.BattleState.Idle);
				Users[0].changeAction(BattleCharacter.ActionState.isAttacking);         
			}
		}

	}
}

using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Collections;
using System.Diagnostics;

[GlobalClass]
public partial class DummyMove1 : MoveBase
{
	[Export] float Speed, AtkMultiplier = 1.5f;
	[Export] Vector2 offset;
	[Export] int Combo1 = 1, Combo2 = 2;
	public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
	{
		base.Effect(Users,Targets);
		
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

		Users[0].changeCombo(Combo2);
		Users[0]._ReturnToIdle+=idle;
		Users[0].StatMultiplier[0]+=AtkMultiplier;

		Tween tween = Users[0].CreateTween();
		//Users[0]._ReturnToIdle+=End;
		Vector2 TargetPosition = Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-Users[0].Character.Base.BattleOffset;
		tween.TweenProperty(Users[0].GetParent(),"position",TargetPosition+offset*Dir,1/Speed);
		tween.TweenCallback(Callable.From(()=>Users[0].changeState(BattleCharacter.BattleState.Attacking)));
		tween.TweenCallback(Callable.From(()=>Users[0].Character.ShowTextLabel($"{Users[0].Character.Key}",Users[0].Character.Base.TextEffectColor)));
		tween.TweenInterval(0.5);
		tween.TweenCallback(Callable.From(Auto));

		tween.Finished+=tween.Kill;

		void Auto(){
			if(Users[0].battleState!=BattleCharacter.BattleState.Idle && Users[0].actionState!=BattleCharacter.ActionState.isAttacking){
				idle(Users[0]);
				Users[0].changeAction(BattleCharacter.ActionState.isAttacking);         
			}
			else{
				Users[0].Character.ChangeWP(5);
			}
		}

		void idle(BattleCharacter character){
			character.battleState=BattleCharacter.BattleState.Idle;
			Users[0].changeCombo(Combo1);
			Users[0].StatMultiplier[0]-=AtkMultiplier;
			//Users[0].MultiplierAtk.RemoveAt(Users[0].MultiplierAtk.Count-1);
			//Users[0].TimerAtk.RemoveAt(Users[0].MultiplierAtk.Count-1);
			Users[0]._ReturnToIdle-=idle;
		}
		/*void End(){
			a++;
			if(a>=1){
				Users[0].GetParent<Node2D>().Rotation=RotationAux;
				Users[0].GetParent<Node2D>().Scale=ScaleAux;
				Users[0]._ReturnToIdle-=End;
				Users[0].Hitbox._Hit-=onHit;                
				BattleManager.instance.CallDeferred("EndMove");
				tween.Kill();                
			}
		}*/

	}

}

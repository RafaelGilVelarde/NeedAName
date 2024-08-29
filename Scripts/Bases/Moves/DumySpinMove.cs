using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
[GlobalClass]

public partial class DumySpinMove : MoveBase
{
[Export] float MaxSpeed=20;
[Export] int Combo = 1;
	[Export] float offset;
	public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
	{
		Debug.WriteLine(Users[0].Character.Base.Name+"vs"+Targets[0].Character.Base.Name);
		base.Effect(Users,Targets);
		float Radius= 0.5f*(Mathf.Abs(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X)-offset);
		Vector2 OriginPos=Users[0].GlobalPosition;
		Vector2 TargetPos=Targets[0].GlobalPosition;
		Vector2 MiddlePos=(OriginPos+TargetPos)/2;
		Vector2 Spin=new Vector2(1,1);

		Tween tween = Users[0].CreateTween();
		
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
		Spin=new Vector2(Spin.X*Dir,1);

		Users[0].Combo=Combo;
		Users[0].changeAction(BattleCharacter.ActionState.isAttacking);
		Targets[0].Controllable=true;
		Targets[0].changeState(BattleCharacter.BattleState.Defending);

		tween.TweenMethod(Callable.From((float w)=>Rotate2(w,MiddlePos,Radius,(Node2D)Users[0].GetParent())), 0.5, 4f, 2.0);

		tween.Finished+=End;
		tween.Finished+=tween.Kill;
		void End(){
			Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",true);
			Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",false);
		}

		void Rotate2(float w, Vector2 center,float radius,Node2D obj){
    		obj.Position = center + Spin*( Vector2.FromAngle(w * Mathf.Tau) * radius);
		}
	}
}

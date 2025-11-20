using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
[GlobalClass]

public partial class DumySpinMove : MoveBase
{
[Export] float SpinCount = 2;
[Export] int Combo = 1;
	[Export] Vector2 offset = new Vector2(7,7);
	public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
	{
		base.Effect(Users,Targets);
		float Radius= 0.5f*(Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-Users[0].GlobalPosition-Users[0].BattleOffset-offset).Length();
		Vector2 OriginPos=Users[0].GlobalPosition-Users[0].BattleOffset;
		Vector2 TargetPos=Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-offset;
		Vector2 MiddlePos=(OriginPos+TargetPos)/2;
		Vector2 Spin=new Vector2(1,1);

		float Angle = MiddlePos.AngleToPoint(OriginPos);
		Debug.WriteLine("Position: " + OriginPos);
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
		Vector2 AuxDebug = MiddlePos + Spin * (-Vector2.FromAngle(Angle * Mathf.Tau) * Radius);


		Users[0].changeCombo(Combo);
		Users[0].changeAction(BattleCharacter.ActionState.isAttacking);
		Targets[0].Controllable=true;
		Targets[0].changeState(BattleCharacter.BattleState.Defending);

		tween.TweenInterval(0.5);
		tween.TweenMethod(Callable.From((float w)=>Rotate2(w,MiddlePos,Radius,(Node2D)Users[0].GetParent())), -Angle/Mathf.Tau, (-Angle/Mathf.Tau)+SpinCount*2, MoveTime-1);

		tween.Finished+=End;
		tween.Finished+=tween.Kill;
		void End(){
			Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",true);
			SceneTreeTimer AnimEndTimer = Users[0].GetTree().CreateTimer(0.1,true,false);
            AnimEndTimer.Timeout += () =>
            {
				Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",false);                
            };
		}
		void Rotate2(float w, Vector2 center,float radius,Node2D obj){
    		obj.Position = center + Spin*( -Vector2.FromAngle(w*Mathf.Tau) * radius);
		}
	}
}

using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;
[GlobalClass]

public partial class DumyShootMove : MoveBase
{
[Export] float Speed, Middle=2, Timer2, Height=50;
[Export] int Combo = 1;
[Export] String Tag;
[Export] PackedScene Proyectile;
	[Export] Vector2 offset;
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

	
		
		BezierProyectile proyectile=Proyectile.Instantiate<BezierProyectile>();
		Hitbox Hitbox= (Hitbox)proyectile.GetChild(0);
		Hitbox.AddToGroup(Tag);
		Users[0]._Shoot+=shoot;
		Hitbox._Hit+=onHit;
        Hitbox._Block+=blocked;
		Hitbox.Character=Users[0];

        Timer timer = new Timer
        {
            OneShot = true,
            WaitTime = Timer2
        };
        timer.Timeout+=End;
		Users[0].GetTree().CurrentScene.AddChild(timer);
		timer.Start();
		Targets[0].Controllable=true;           
		Users[0].Combo=Combo;
		Users[0].changeAction(BattleCharacter.ActionState.isAttacking);
		Targets[0].Controllable=true;
		Targets[0].changeState(BattleCharacter.BattleState.Defending);

        void onHit(BattleCharacter Target){
			Hit(Users[0],Target);
			proyectile.Hit();
		}		
		void blocked(BattleCharacter Target){
			Block(Target);
		}
		void shoot(BattleCharacter User){
			if(proyectile.CanStart){
				proyectile.PosStart=Users[0].ShootNode.GlobalPosition;
				proyectile.PosEnd=Targets[0].GlobalPosition;
				float MiddleX =proyectile.PosStart.X+((proyectile.PosEnd.X-proyectile.PosStart.X)/Middle);
				proyectile.PosMiddle=new Vector2(MiddleX,Users[0].GlobalPosition.Y-Height);

				proyectile.GlobalPosition=User.ShootNode.GlobalPosition;
				proyectile.GlobalScale=User.GlobalScale;
				proyectile.GlobalRotation=User.GlobalRotation;
				proyectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Hit",false);
				proyectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Start",true);			
				if(proyectile.GetParent()==null){
					User.GetTree().CurrentScene.AddChild(proyectile);
				}
				proyectile.Start();
			}
		}
		void End(){
			proyectile.Ended=true;
			Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",true);
			Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",false);
			Users[0]._Shoot-=shoot;
			proyectile.Hit();				
			timer.QueueFree();
		}
	}
}

using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class RaiseStatWithEffectMove : RaiseStatMove
{
    [Export] float Middle=2, Timer2, Height=50, Length = 50, speed = 2;
    [Export] int Combo = 1;
    [Export] string Tag;
    [Export] PackedScene Proyectile;
    [Export] Vector2 offset;
	[Export] BattleCharacter.ActionState State;


    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
		Array<Vector2> ScaleAux=new Array<Vector2>();
		Array<float> RotationAux=new Array<float>();
        for(int i=0;i<Users.Count;i++){
            ScaleAux.Add(Users[i].GlobalScale);      
            RotationAux.Add(Users[i].GlobalRotation);
        }

        
        Array<Vector2> TargetScaleAux=new Array<Vector2>();
		Array<float> TargetRotationAux=new Array<float>();
        for(int i=0;i<Targets.Count;i++){
                TargetScaleAux.Add(Targets[i].GlobalScale);     
            TargetRotationAux.Add(Targets[i].GlobalRotation);
        }

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
        SceneTreeTimer timer=Users[0].GetTree().CreateTimer(MoveTime,true,true);

        Users[0].changeAction(State);
		Users[0].changeCombo(Combo);

		BezierProyectile proyectile=Proyectile.Instantiate<BezierProyectile>();
		Hitbox Hitbox= (Hitbox)proyectile.GetChild(0);
		Hitbox.AddToGroup(Tag);
		Users[0]._Shoot+=shoot;
		Hitbox.AreaEntered+=onHit;
        timer.Timeout+=End;

        void shoot(BattleCharacter User){
			if(proyectile.CanStart){
				proyectile.PosStart=Users[0].ShootNode.GlobalPosition;
				proyectile.PosEnd=Targets[0].GlobalPosition+new Vector2(offset.X*Dir,offset.Y);
				proyectile.Target = Targets[0];
				float MiddleX =proyectile.PosStart.X+((proyectile.PosEnd.X-proyectile.PosStart.X)/Middle);
				float MiddleY =proyectile.PosStart.Y+((proyectile.PosEnd.Y-proyectile.PosStart.Y)/Middle);
				proyectile.PosMiddle=new Vector2(MiddleX + Length,MiddleY - Height);

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
        
        void onHit(Area2D Target){
			if((Hitbox.IsInGroup("PlayerHitbox") && Target.IsInGroup("PlayerHurtbox"))||(Hitbox.IsInGroup("EnemyHitbox")&&Target.IsInGroup("EnemyHurtbox"))){
				BattleCharacter TargetChar=Target.GetNode<BattleCharacter>("..");
				Stats.RaiseStat(TargetChar);
				proyectile.Hit();
				proyectile.Ended = true;
				Hitbox.AreaEntered-=onHit;
				timer.TimeLeft = 0;
			}
		}		

        void End(){
			SceneTreeTimer sceneTreeTimer = Users[0].GetTree().CreateTimer(0.5,true,true);
			sceneTreeTimer.Timeout+=()=>{
				Users[0]._Shoot-=shoot;
				for(int i=0;i<Users.Count;i++){
					Users[i].GetParent<Node2D>().Rotation=RotationAux[i];
					Users[i].GetParent<Node2D>().Scale=ScaleAux[i];
				}
				for(int i=0;i<Targets.Count;i++){
					Targets[i].GetParent<Node2D>().Scale=TargetScaleAux[i];
					Targets[i].GetParent<Node2D>().Rotation=TargetRotationAux[i];
				}

				BattleManager.instance.CallDeferred("EndMove");
			};
		}
    }

}

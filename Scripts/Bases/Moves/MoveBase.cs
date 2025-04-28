using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
[GlobalClass]

public partial class MoveBase : Resource
{
    public enum Type{
        Physical,
        Special,
        Status,
        Recovery,
        Revive
    }
    [Export] public Type type;
    [Export]public int Power, UserAmount, TargetAmount, WP, Cost;
    [Export]public bool TargetsParty;
    [Export]public String Name;

    [Export] protected float MoveTime;
    protected SceneTreeTimer MainTimer;

    public virtual void Effect(Array<BattleCharacter> Users, Array<BattleCharacter>Targets){
        for(int i=0;i<Users.Count;i++){
            Users[0].Hitbox._Hit+=onHit;
            Users[0].Hitbox._Block+=blocked;
            Users[0].Hitbox._WP+=Graze;
        }

        
		Array<Vector2> ScaleAux=new Array<Vector2>();
		Array<float> RotationAux=new Array<float>();
        for(int i=0;i<Users.Count;i++){
            ScaleAux.Add(Users[i].Scale);      
            RotationAux.Add(Users[i].Rotation);
            Users[i].AddAttack();
        }

        
        Array<Vector2> TargetScaleAux=new Array<Vector2>();
		Array<float> TargetRotationAux=new Array<float>();
        for(int i=0;i<Targets.Count;i++){
            TargetScaleAux.Add(Targets[i].Scale);     
            TargetRotationAux.Add(Targets[i].Rotation);
        }

		MainTimer=Users[0].GetTree().CreateTimer(MoveTime,true,true);
   		//timer.TweenInterval(MoveTime);
		MainTimer.Timeout+=End;
		//timer.Finished+=timer.Kill;

        void onHit(BattleCharacter Target){
            Debug.WriteLine("Hit");
			Hit(Users[0],Target);
		}		
		void blocked(BattleCharacter Target){
			Block(Target);
		}
        void Graze(BattleCharacter Target){
            WPGraze(Target);
        }
        void End(){
            for(int i=0;i<Users.Count;i++){
                Users[i].Hitbox._Hit-=onHit;
                Users[i].Hitbox._WP-=Graze;
                Users[i].Hitbox._Block-=blocked;
                Users[i].RemoveAttack();
                Users[i].GetParent<Node2D>().Rotation=RotationAux[i];
                Users[i].GetParent<Node2D>().Scale=ScaleAux[i];
            }
            for(int i=0;i<Targets.Count;i++){
                Targets[i].GetParent<Node2D>().Scale=TargetScaleAux[i];
		        Targets[i].GetParent<Node2D>().Rotation=TargetRotationAux[i];
            }

			BattleManager.instance.CallDeferred("EndMove");
		}

    }
    public virtual void DealDamage(BattleCharacter Users, BattleCharacter Targets,bool blocked){
        for (int i=0;i<Targets.Character.Equipment.Count;i++){
            Targets.Character?.Equipment[i]?.GetHitEffect(Targets,this);
        }
        int Calc;
        if(!blocked){
            switch (type){
                case Type.Physical:
                    Calc = Mathf.RoundToInt(Users.Character.TotalStats.Atk*Power*Users.MultiplyAtk()-Targets.Character.TotalStats.Def*Targets.MultiplyDef())*-1;
                    Calc = (int)Mathf.Clamp(Calc,-Mathf.Inf,0);
                    Targets.Character.ChangeHP(Calc);
                break;
                case Type.Special:
                    Calc = Mathf.RoundToInt(Users.Character.TotalStats.SpAtk*Power*Users.MultiplySpAtk()-Targets.Character.TotalStats.SpDef*Targets.MultiplySpDef())*-1;
                    Calc = (int)Mathf.Clamp(Calc,-Mathf.Inf,0);
                    Targets.Character.ChangeHP(Calc);
                break;
                case Type.Recovery:
                    Targets.Character.ChangeHP(Mathf.RoundToInt(Users.Character.TotalStats.SpAtk*Power*Users.MultiplySpAtk()));
                break;
            }
        }
        else{
             switch (type){
                case Type.Physical:
                    Targets.Character.ChangeHP(0);
                break;
                case Type.Special:
                    Targets.Character.ChangeHP(0);
                break;
                case Type.Recovery:
                    Targets.Character.ChangeHP(Mathf.RoundToInt(0));
                break;
            }
            Targets.Character.ChangeWP(WP);           
        }
    }
    
	public virtual void Hit(BattleCharacter User, BattleCharacter Target){
			Tween HitTween=User.CreateTween();
			HitTween.TweenInterval(1/Engine.GetFramesPerSecond());
			Callable check=Callable.From(()=>Check());

			HitTween.TweenCallback(check);
			HitTween.Finished+=HitTween.Kill;
			void Check(){
				if(!Target.BlockedEnemy){
					DealDamage(User,Target,false);
				}
				else{
					DealDamage(User,Target,true);
				}
			}
	}
    public virtual void Block(BattleCharacter Target){
			Target.BlockedEnemy=true;
	}
    public virtual void WPGraze(BattleCharacter Target){
        Debug.WriteLine("Graze");
			Target.Character.ChangeWP(WP);
	}

}

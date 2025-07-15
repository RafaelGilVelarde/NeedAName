using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class SpinSpamMove : MoveBase
{
    [Export] Vector2 offset;
    [Export] float AtkMultiplier, Speed = 1;
    [Export] int Combo = 1;
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
        base.Effect(Users, Targets);
		BattleScene CurrentScene = BattleManager.instance.Scene;


        int Spam = 0;
        SceneTreeTimer Timer = Users[0].GetTree().CreateTimer(MoveTime-0.05,true,true,true);
        float Dir=(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X)/Mathf.Abs(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X);


        bool AttackAvailable = true;
        bool Failed = false;
        bool MoveEnded = false;
        Users[0].Hitbox._Hit+=SpamInterval;
        Timer.Timeout+=End;
        Users[0]._DoAction+=Check;

		if(Dir==1||Dir==-1){
			Users[0].GetParent<Node2D>().Rotation=0;
			Users[0].GetParent<Node2D>().Scale=new Vector2(Dir,1);
			
			Targets[0].GetParent<Node2D>().Rotation=0;
			Targets[0].GetParent<Node2D>().Scale=new Vector2(-Dir,1);
		}		
		else{
			Dir=Users[0].GetParent<Node2D>().Scale.Y;
		}
        //Users[0]._ReturnToIdle+=SpamInterval;
        void Check(){

            if(!MoveEnded){

                if(AttackAvailable){
                    Attack();
                }
                else{
                    Failed = true;
                }
            }
            AttackAvailable = false;
        }
        void SpamInterval(BattleCharacter AuxChar){
            Users[0].changeState(BattleCharacter.BattleState.Attacking);
            RandomNumberGenerator RNG=new RandomNumberGenerator();
            float YOff=RNG.RandfRange(0,offset.Y);
            Vector2 AuxOffset = new Vector2(offset.X*Dir,YOff);

            Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/Hit",true);
            Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/Windup",false);
            
            Tween Tween = Users[0].CreateTween();
            Vector2 TargetPosition = Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-Users[0].BattleOffset-AuxOffset;
            Tween.TweenProperty(Users[0].GetParent(),"position",TargetPosition,1/Speed);
            
            Tween.Finished+=()=>{
                if(!Failed && Timer.TimeLeft>((2/Speed)+0.5)){
                    AttackAvailable = true;
                    Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/Hit",false);

                    Users[0].Character.ShowTextLabel($"{Users[0].Character.Key}",Users[0].Character.Base.TextEffectColor);
                    SceneTreeTimer AuxTimer= Users[0].GetTree().CreateTimer(0.5,true,true,true);
                    AuxTimer.Timeout+=()=>{
                        if(AttackAvailable){
                            AttackAvailable = false;
                            //Users[0]._DoAction-=Attack;
                            Timer.TimeLeft = 0;
                        }
                        };                    
                }
                else{
                    Timer.TimeLeft = 0;
                }
                Tween.Kill();
            };
        }

        /*void IncreaseSpam(){
            Users[0]._DoAction-=IncreaseSpam;
            Users[0].changeCombo((Spam%MaxCombo)+1);
            Debug.WriteLine("Spam: "+(Spam%MaxCombo)+1);
            Spam++;
            Users[0].changeState(BattleCharacter.BattleState.Idle);
            if(MultIndex ==  Users[0].MultiplierAtk.Count){
                Users[0].AddAtkMultiplier(AtkMultiplier,1);
            }
            else{
                Users[0].MultiplierAtk[MultIndex]*=AtkMultiplier;
            }
        }   */

		//Users[0]._ReturnToIdle+=End;
        void Attack(){
            Users[0].Character.ChangeWP(WP);
            Users[0].changeState(BattleCharacter.BattleState.Idle);
            Spam++;
            Debug.WriteLine("Spam Increased: "+Spam);
            Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/Windup",true);
            //Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/Windup",false);
            //Users[0]._DoAction-=Attack;
            Users[0].changeState(BattleCharacter.BattleState.Idle);
            
            Tween Tween = Users[0].CreateTween();

            Vector2 TargetPosition = Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-Users[0].BattleOffset;

            if (CurrentScene.Horizontal)
			{
				float FloorOffset = CurrentScene.EnemyFloorY[Targets[0].PosIndex] - CurrentScene.PartyFloorY[Users[0].PosIndex];
				TargetPosition = new Vector2(TargetPosition.X, Users[0].GlobalPosition.Y + FloorOffset);
			}
            Tween.TweenProperty(Users[0].GetParent(),"position",TargetPosition,1/Speed);
            /*Tween.Finished+=HitSignal;
            void HitSignal(){
                SpamInterval();
                Tween.Kill();
            }*/
            //tween.TweenCallback(Callable.From(()=>SpamInterval(Users[0])));
        }
        

        //Attack();
        Users[0].changeState(BattleCharacter.BattleState.Attacking);
		

        void End(){
            if(!MoveEnded){

                MoveEnded = true;
                AttackAvailable = false;
                Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",true);
                Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",false);
                Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/Windup",false);
                Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/Hit",false);

                Users[0].Hitbox._Hit-=SpamInterval;
                Timer.Timeout-=End;
                Users[0]._DoAction-=Check;

                Users[0].changeState(BattleCharacter.BattleState.Idle);
                if(MainTimer.TimeLeft>0.1){
                    SceneTreeTimer timer = Users[0].GetTree().CreateTimer(0.1,true,true,true);
                    timer.Timeout+=()=>{
                        MainTimer.TimeLeft = 0;
                        Debug.WriteLine("Time: "+MainTimer.TimeLeft);
                    };
                }
            }
        }
    }
}

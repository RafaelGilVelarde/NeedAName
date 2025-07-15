using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class SpamMove : MoveBase
{
    [Export] Vector2 offset;
    [Export] float AtkMultiplier, Speed = 1;
    [Export] int MaxCombo = 2;
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
        base.Effect(Users, Targets);
		BattleScene CurrentScene = BattleManager.instance.Scene;


        int Spam = 0;
        float OriginMultiplier = Users[0].StatMultiplier[0];
        Users[0]._ReturnToIdle+=SpamInterval;

        void SpamInterval(BattleCharacter character){
            SceneTreeTimer AuxTimer= character.GetTree().CreateTimer(0.2,true,true,true);
            AuxTimer.Timeout+=()=>{
            character.changeState(BattleCharacter.BattleState.Attacking);
            character.Character.ShowTextLabel($"{character.Character.Key}",character.Character.Base.TextEffectColor);
            int auxSpam = Spam;
            character._DoAction+=IncreaseSpam;
            SceneTreeTimer Timer= character.GetTree().CreateTimer(0.5,true,true,true);
            Timer.Timeout+=()=>{
                if(auxSpam == Spam){
                    character._DoAction-=IncreaseSpam;
                    End();
                }
                };
            };
        }

        void IncreaseSpam(){
            Users[0]._DoAction-=IncreaseSpam;
            Users[0].changeCombo((Spam%MaxCombo)+1);
            Spam++;
            Users[0].changeState(BattleCharacter.BattleState.Idle);
            Users[0].StatMultiplier[0]*=AtkMultiplier;
        }   


        SceneTreeTimer Timer = Users[0].GetTree().CreateTimer(MoveTime-0.5,true,true,true);
        Timer.Timeout+=End;

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
        		Tween tween = Users[0].CreateTween();
		//Users[0]._ReturnToIdle+=End;
		Vector2 TargetPosition = Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-Users[0].BattleOffset;
        if (CurrentScene.Horizontal)
		{
				float FloorOffset = CurrentScene.PartyFloorY[Targets[0].PosIndex] - CurrentScene.EnemyFloorY[Users[0].PosIndex];
				TargetPosition = new Vector2(TargetPosition.X, Users[0].GlobalPosition.Y + FloorOffset);
		}
		tween.TweenProperty(Users[0].GetParent(),"position",TargetPosition+offset*Dir,1/Speed);
		tween.TweenCallback(Callable.From(()=>SpamInterval(Users[0])));
		
		tween.Finished+=tween.Kill;

        void End(){
            Users[0].changeState(BattleCharacter.BattleState.Idle);
            Users[0].StatMultiplier[0] = OriginMultiplier; 
            Users[0]._ReturnToIdle-=SpamInterval;
        }
    }


}

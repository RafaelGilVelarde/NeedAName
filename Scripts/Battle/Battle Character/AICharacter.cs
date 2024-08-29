using Godot;
using System;
using System.Diagnostics;

public partial class AICharacter : BattleCharacter
{
    public override void StartChoosingMove(){
        Moves move=((EnemyCharacterBase)Character.Base).MoveChoosing(this);
        MoveUsed=move;
        if(move.Base.UserAmount>1){
            StartChoosingUsers(move);
        }
        else{
            StartChoosingTarget(move);
        }
	}
	public override void StartChoosingUsers(Moves move){

	}

	public override void StartChoosingTarget(Moves move){
        ((EnemyCharacterBase)Character.Base).TargetChoosing(move,this);
        UseMove(MoveUsed);
	}
    public override void TurnOffBattle()
    {
		ProcessMode=ProcessModeEnum.Disabled;
		ClearTimers();
		AnimatorTree.Active=false;
		AnimatorTree.Set("parameters/conditions/Ended",false);
		Character._GetHit-=GetHit;
		Character._Die-=Die;
        if(Character.status!=Character.Status.KO){
		    Hide();
		    Overworld.BattleEnd();
        }
   }
}

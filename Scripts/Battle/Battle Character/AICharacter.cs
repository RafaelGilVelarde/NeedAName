using Godot;
using System;
using System.Diagnostics;

public partial class AICharacter : BattleCharacter
{
    public override void _Ready()
    {
        base._Ready();
    }
    public override void StartChoosingMove(){
        Moves move=((EnemyCharacterBase)Character.Base).AI.MoveChoosing(this);
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
        ((EnemyCharacterBase)Character.Base).AI.TargetChoosing(move,this);
        UseMove(MoveUsed);
	}
    public override void TurnOnBattle()
    {
        ((EnemyCharacter)Character).AI.Clear();
        base.TurnOnBattle();
    }
    public override void TurnOffBattle()
    {
        ((EnemyCharacter)Character).AI.Clear();
		ClearStatMultiplier();
		Character._GetHit-=GetHit;
		Character._Die-=Die;
        Character._ChangeHP-=ShowChangeHPBar;

   }
    public override void ReturnToOverworld()
    {
		ProcessMode=ProcessModeEnum.Disabled;
		AnimatorTree.Active=false;
		AnimatorTree.Set("parameters/conditions/Ended",false);
        if(Character.status.Base.Name!="KO"){
		    Hide();
		    Overworld.BattleEnd();
        }
        TurnOffBattle();
    }
}

using Godot;
using System;
using System.Diagnostics;

[GlobalClass]

public partial class PartyCharacters : Character
{
	[Export] int Exp;
	[Export] int NextLevelExp;
	public void ChangeKey(String Action, InputEvent Event){
		InputMap.EraseAction(Action);
		InputMap.ActionAddEvent(Action,Event);
	}
	public void GainExp(int GainedExp){
		Exp+=GainedExp;
		Debug.WriteLine("EXP Gained: "+GainedExp+" NextLVEXP: "+NextLevelExp);
		Debug.WriteLine("Current EXP: "+Exp);
		if(Exp>NextLevelExp){
			LevelUp();
		}
	}

	public void LevelUp(){
		PartyCharacterBase Aux = (PartyCharacterBase)Base;
		stats.Lv++;
		NextLevelExp = (int)Mathf.Pow(stats.Lv/Aux.ExpSpeed,Aux.ExpDistance);
		Debug.WriteLine("Levelup: "+stats.Lv+" NextLVEXP: "+NextLevelExp);
		SetStats();
		if(Exp>=NextLevelExp){
			LevelUp();
		}
	}
	// Called when the node enters the scene tree for the first time.


	// Called every frame. 'delta' is the elapsed time since the previous frame.

}

using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class CharacterButtons : Control
{
	public BattleCharacter Character;
	public bool isTarget;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FocusEntered+=OnFocusEntered;
		FocusExited+=OnFocusExited;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void SetBattleCharacter(BattleCharacter character){
		Character=character;
		Position=character.Position;
		
	}
	void OnFocusEntered(){
		BattleManager.instance.CurrentCharacterButton=this;
		Character.MainSprite.Modulate=Colors.IndianRed;
		Character.ShowChangeHPBar(Character.Character.stats.HP);
		Character.ShowStatsMods();
		
		if (isTarget)
		{
			BattleManager.instance.Turns.UpdateAuxTurns(BattleManager.instance.UserCharacters[0].MoveUsed,null,Character);			
		}
		else
		{
			BattleManager.instance.Turns.UpdateAuxTurns(BattleManager.instance.UserCharacters[0].MoveUsed,Character,null);				
		}
		BattleManager.instance.Turns.ColorFocus(Colors.Red,new Array<BattleCharacter>() {Character});				
	}
	void OnFocusExited(){
		Character.MainSprite.Modulate=Colors.White;
		Character.HideChangeHPBar();
		Character.HideStatsMods();
		BattleManager.instance.Turns.ColorFocus(Colors.White,new Array<BattleCharacter>() {Character});				
	}
}

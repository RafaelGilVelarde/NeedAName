using Godot;
using System;
using System.Diagnostics;

public partial class CharacterButtons : Control
{
	public BattleCharacter Character;
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
	}
	void OnFocusExited(){
		Character.MainSprite.Modulate=Colors.White;
	}
}

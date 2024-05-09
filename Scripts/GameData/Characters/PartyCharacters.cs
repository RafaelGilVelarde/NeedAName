using Godot;
using System;

[GlobalClass]

public partial class PartyCharacters : Character
{
	public void ChangeKey(String Action, InputEvent Event){
		InputMap.EraseAction(Action);
		InputMap.ActionAddEvent(Action,Event);
	}
	// Called when the node enters the scene tree for the first time.


	// Called every frame. 'delta' is the elapsed time since the previous frame.

}

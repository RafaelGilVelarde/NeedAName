using Godot;
using System;
using System.Diagnostics;

public partial class MoveButtons : Control
{
	public Moves move;
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
	void OnFocusEntered(){
		BattleManager.instance.CurrentMoveButton=this;
		Modulate=Colors.Yellow;
	}
	void OnFocusExited(){
				Modulate=Colors.White;
	}
}

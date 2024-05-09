using Godot;
using System;

public partial class ItemButtons : Control
{
	public Consumables item;
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
		BattleManager.instance.CurrentItemButton=this;
	}
	void OnFocusExited(){
	}
}

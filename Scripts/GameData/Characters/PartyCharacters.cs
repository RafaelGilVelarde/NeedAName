using Godot;
using System;
using System.Diagnostics;

[GlobalClass]

public partial class PartyCharacters : Character
{
	[Export] public int Exp { get; private set; }
	[Export] public int NextLevelExp { get; private set; }
	[Export] public int PastLevelExp { get; private set; }

	public void ChangeKey(String Action, InputEvent Event)
	{
		InputMap.EraseAction(Action);
		InputMap.ActionAddEvent(Action, Event);
	}
	public void GainExp(int GainedExp)
	{
		Exp += GainedExp;
		Debug.WriteLine(Base.Name+":"+"EXP Gained: " + GainedExp + " NextLVEXP: " + NextLevelExp);
		Debug.WriteLine(Base.Name+":"+"Current EXP: " + Exp);
		if (Exp > NextLevelExp)
		{
			LevelUp();
			SetTotalStats();
		}
	}

	public void LevelUp()
	{
		PastLevelExp = NextLevelExp;
		PartyCharacterBase Aux = (PartyCharacterBase)Base;
		stats.Lv++;
		NextLevelExp = Aux.ExpForLevel[stats.Lv - 1];
		Debug.WriteLine(Base.Name+":"+"Levelup: " + stats.Lv + " NextLVEXP: " + NextLevelExp);
		SetStats();
		if (Exp >= NextLevelExp)
		{
			LevelUp();
		}
	}


	// Called when the node enters the scene tree for the first time.


	// Called every frame. 'delta' is the elapsed time since the previous frame.

}

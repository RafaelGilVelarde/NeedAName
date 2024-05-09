using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class MoveList : StuffList
{
	[Export]public Array<MoveButtons> Buttons;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void FillButtons(int Start,ScrollList.StartEnd StartEnd)
    {
		pointerStart=Start;
		if(StartEnd!=ScrollList.StartEnd.Regular){
			if(pointerStart>=0 &&pointerStart+Buttons.Count<=character.Character.Moves.Count){
				Fill(Start,StartEnd);
			}
		}
		else{
			Fill(Start,StartEnd);
		}
		if(pointerStart+activeButtons>character.Character.Moves.Count){
			pointerStart-=1;
			Buttons[activeButtons-1].GrabFocus();
		}
		if(pointerStart<0){
			pointerStart=0;
		}
			if(StartEnd==ScrollList.StartEnd.Start||StartEnd==ScrollList.StartEnd.Regular){
				Buttons[0].GrabFocus();
			}
    }
    public override void ClearAll()
    {
		for(int i=0;i<Buttons.Count;i++){
				Buttons[i].move=null;
				Buttons[i].Hide();
				Buttons[i].GetChild(0).GetNode<RichTextLabel>(".").Text="AAA";
			}
			activeButtons=0;
    }
	void Fill(int Start,ScrollList.StartEnd StartEnd){
		activeButtons=0;
		for(int i=0;i<Buttons.Count;i++){
					if(i+Start<character.Character.Moves.Count){
						Buttons[i].move=character.Character.Moves[i+Start];
						Buttons[i].Show();
						Buttons[i].GetChild(0).GetNode<RichTextLabel>(".").Text=Buttons[i].move.Base.Name;
						activeButtons++;
						if(StartEnd==ScrollList.StartEnd.End){
							Buttons[i].GrabFocus();
						}
					}
					else{
						Buttons[i].move=null;
						Buttons[i].Hide();
					}
				}
	}
}

using Godot;
using Godot.Collections;
using System;

public partial class ItemList : StuffList
{
	[Export]public Array<ItemButtons> Buttons;
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
		if(pointerStart>=0 &&pointerStart+Buttons.Count<character.Character.items.Count){
			for(int i=0;i<Buttons.Count;i++){
				if(i+Start<character.Character.items.Count){
					if(character.Character.items[i+Start].Base.type==ItemBase.Type.Consumable){
						Buttons[i].item= (Consumables)character.Character.items[i+Start];
						Buttons[i].Show();
						activeButtons++;
						if(StartEnd==ScrollList.StartEnd.End){
							Buttons[i].GrabFocus();
						}
					}
				}
				else{
					Buttons[i].item=null;
					Buttons[i].Hide();
				}
			}
		}
				if(pointerStart<0){
			pointerStart=0;
		}
		if(pointerStart+activeButtons>character.Character.items.Count){
			pointerStart-=1;
			Buttons[activeButtons-1].GrabFocus();
		}
				if(StartEnd==ScrollList.StartEnd.Start||StartEnd==ScrollList.StartEnd.Regular){
					Buttons[0].GrabFocus();
				}
    }
    public override void ClearAll()
    {
		activeButtons=0;
		for(int i=0;i<Buttons.Count;i++){
				Buttons[i].item=null;
				Buttons[i].Hide();
			}
    }
}

using Godot;
using Godot.Collections;
using System;

public partial class ItemList : StuffList
{
	[Export]public Array<ItemButtons> Buttons;
	[Export] VBoxContainer AmountList;
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
		Array<Items> items = GameManager.Instance.Data.items[0].items;
		pointerStart=Start;
		if(StartEnd!=ScrollList.StartEnd.Regular){
			if(pointerStart>=0 &&pointerStart+Buttons.Count<items.Count){
				Fill(pointerStart,StartEnd);
			}
		}
		else{
			Fill(pointerStart,StartEnd);
		}
				if(pointerStart<0){
			pointerStart=0;
		}
		if(pointerStart+activeButtons>items.Count){
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
	void Fill(int Start,ScrollList.StartEnd StartEnd){
		activeButtons = 0;
		Array<Items> items = GameManager.Instance.Data.items[0].items;
		for(int i=0;i<Buttons.Count;i++){
			if(i+Start<items.Count){
					if(items[i+Start].Base.type==ItemBase.Type.Consumable){
						Buttons[i].item= (Consumables)items[i+Start];
						Buttons[i].GetChild(0).GetNode<RichTextLabel>(".").Text=Tr(Buttons[i].item.Base.Name);
						AmountList.GetChild<RichTextLabel>(i+1).Show();
						AmountList.GetChild<RichTextLabel>(i+1).Text = $"x{Buttons[i].item.Amount}";
						Buttons[i].Show();
						activeButtons++;
						if(StartEnd==ScrollList.StartEnd.End){
							Buttons[i].GrabFocus();
						}
					}
				}
				else{
					AmountList.GetChild<RichTextLabel>(i+1).Hide();
					Buttons[i].item=null;
					Buttons[i].Hide();
				}
			}
	}

}

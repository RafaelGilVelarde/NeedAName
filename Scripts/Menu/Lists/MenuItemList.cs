using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class MenuItemList : StuffList
{
    [Export] public Array<MenuItemButtons> Buttons;
    [Export] public ItemBase.Type ItemType;
    [Export] Array<Items> items;
    public override void FillButtons(int Start,ScrollList.StartEnd StartEnd)
    {
        items = GameManager.Instance.Data.items[(int)ItemType].items;
		pointerStart=Start;
		if(StartEnd!=ScrollList.StartEnd.Regular){
			if(pointerStart>=0 &&pointerStart+Buttons.Count<=items.Count){
				Fill(Start,StartEnd);
			}
		}
		else{
			Fill(Start,StartEnd);
		}
		if(pointerStart+activeButtons>items.Count){
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
				Buttons[i].item=null;
				Buttons[i].Hide();
				Buttons[i].Text="AAA";
			}
			activeButtons=0;
    }
	void Fill(int Start,ScrollList.StartEnd StartEnd){
		activeButtons=0;
		for(int i=0;i<Buttons.Count;i++){
					if(i+Start<items.Count){
						Buttons[i].item=items[i+Start];
						Buttons[i].Show();
						Buttons[i].Text=$"{items[i+Start].Base.Name} x{items[i+Start].Amount}";
						activeButtons++;
						if(StartEnd==ScrollList.StartEnd.End){
							Buttons[i].GrabFocus();
						}
					}
					else{
						Buttons[i].item=null;
						Buttons[i].Hide();
					}
				}
	}
}

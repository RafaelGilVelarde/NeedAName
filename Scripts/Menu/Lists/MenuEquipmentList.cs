using Godot;
using Godot.Collections;
using System;

public partial class MenuEquipmentList : StuffList
{
    [Export] public Array<MenuItemButtons> Buttons;
    [Export] public ItemBase.Type ItemType;
    [Export] public EquipmentType EquipType;
    [Export] Array<Items> items;
	public override void FillButtons(int Start, ScrollList.StartEnd StartEnd)
	{
		MakeItemList();
		pointerStart = Start;
		if (StartEnd != ScrollList.StartEnd.Regular)
		{
			if (pointerStart >= 0 && pointerStart + Buttons.Count <= items.Count)
			{
				Fill(Start, StartEnd);
			}
		}
		else
		{
			Fill(Start, StartEnd);
		}
		if (activeButtons != 0)
		{
			if (pointerStart + activeButtons > items.Count)
			{
				pointerStart -= 1;
				Buttons[activeButtons - 1].GrabFocus();
			}
			if (pointerStart < 0)
			{
				pointerStart = 0;
			}
			if (StartEnd == ScrollList.StartEnd.Start || StartEnd == ScrollList.StartEnd.Regular)
			{
				Buttons[0].GrabFocus();
			}
		}
		else
		{
			GetChild<BaseButton>(1).GrabFocus();
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
						Buttons[i].Text=$"{Tr(items[i+Start].Base.Name)} x{items[i+Start].Amount}";
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
    void MakeItemList(){
        items.Clear();
        Array<Items> Aux = GameManager.Instance.Data.items[(int)ItemType].items;
        for(int i = 0;i<Aux.Count;i++){
            if(((EquipmentBase)Aux[i].Base).EquipType == EquipType){
                items.Add(Aux[i]);
            }
        }
    }
}

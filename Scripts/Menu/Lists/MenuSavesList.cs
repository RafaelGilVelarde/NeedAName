using Godot;
using Godot.Collections;
using System;

public partial class MenuSavesList : StuffList
{
        [Export] public Array<MenuSaveButtons> Buttons;
        Array<DataManager> Saves; 


    public override void FillButtons(int Start,ScrollList.StartEnd StartEnd)
    {
		GameManager.Instance.DisplaySaves();
        Saves = GameManager.Instance.Saves;
		pointerStart=Start;
		if(StartEnd!=ScrollList.StartEnd.Regular){
			if(pointerStart>=0 &&pointerStart+Buttons.Count<=Saves.Count){
				Fill(Start,StartEnd);
			}
		}
		else{
			Fill(Start,StartEnd);
		}
		if(pointerStart+activeButtons>Saves.Count){
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
				Buttons[i].Save=null;
				Buttons[i].Hide();
				Buttons[i].Text="AAA";
			}
			activeButtons=0;
    }
	void Fill(int Start,ScrollList.StartEnd StartEnd){
		activeButtons=0;
		for(int i=0;i<Buttons.Count;i++){
					if(i+Start<Saves.Count){
						if(Saves[i+Start]!=null){
							Buttons[i].Save=Saves[i+Start];
							Buttons[i].Text=$"Save: {i+Start}";							
						}
						else{
							Buttons[i].Save=null;
							Buttons[i].Text="---";									
						}
						Buttons[i].Show();
						activeButtons++;
						if(StartEnd==ScrollList.StartEnd.End){
							Buttons[i].GrabFocus();
						}
					}
					else{
						Buttons[i].Save=null;
						Buttons[i].Hide();
					}
				}
	}
}

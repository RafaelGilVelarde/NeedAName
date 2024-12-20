using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Reflection;

public partial class MenuCharacterChooseList : StuffList
{
    [Export] public Array<MenuCharacterButtons> Buttons;
    [Export] Color FocusOff, FocusOn;
    Array<PartyCharacters>Characters;
    int CurrentPointer;
    public override void _Ready()
    {
        base._Ready();
        foreach(MenuCharacterButtons Button in Buttons){
            Button.FocusEntered+=()=>UpdatePointer(Button.GetIndex());
            Button.FocusEntered+=()=>ChangeColor(Button,FocusOn);
            Button.FocusExited+=()=>ChangeColor(Button,FocusOff);
        }
        
    }
    public override void FillButtons(int Start,ScrollList.StartEnd StartEnd)
    {        
        Characters = GameManager.Instance.Data.Party;
		pointerStart=Start;

		if(StartEnd!=ScrollList.StartEnd.Regular){
			if(pointerStart>=0 &&pointerStart+Buttons.Count<=Characters.Count){
				Fill(Start,StartEnd);
			}
		}
		else{
			Fill(Start,StartEnd);
		}
		if(pointerStart+activeButtons>Characters.Count){
			pointerStart-=1;
			Buttons[activeButtons-1].GrabFocus();
		}
		if(pointerStart<0){
			pointerStart=0;
		}
		if(StartEnd==ScrollList.StartEnd.Start){
			Buttons[0].GrabFocus();
		}
        if(StartEnd==ScrollList.StartEnd.Regular){
            Buttons[CurrentPointer].GrabFocus();
        }
        //Fill(Start,StartEnd);
    }
    void Fill(int Start,ScrollList.StartEnd StartEnd){
        activeButtons=0;
        for(int i = 0;i<Buttons.Count;i++){
            if(i+Start<Characters.Count){
                Buttons[i].character = Characters[i+Start];
                Buttons[i].Show();
                Buttons[i].TextureNormal = (Texture2D)Characters[i+Start].Base.Icon;
                Buttons[i].Visible = true;
                activeButtons++;
				if(StartEnd==ScrollList.StartEnd.End){
					Buttons[i].GrabFocus();
				}
            }
            else{
                Buttons[i].Visible = false;
                Buttons[i].Hide();
            }
        }
        /*Buttons[0].character = Characters[Start%(Characters.Count)];
		Buttons[0].GrabFocus();*/
	}
    public override void ClearAll()
    {
        for(int i=0;i<Buttons.Count;i++){
			Buttons[i].character=null;
			Buttons[i].Hide();
		}
        pointerStart = 0;
        Characters.Clear();
        Buttons[0].character = null;
        activeButtons=0;        
    }

    void UpdatePointer(int Point){
        CurrentPointer = Point;
    }
    void ChangeColor(BaseButton button, Color color){
        button.SelfModulate = color;
    }
}

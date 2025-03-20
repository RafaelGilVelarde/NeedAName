using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class MenuCharacterList : StuffList
{
    [Export] public Array<MenuCharacterButtons> Buttons;
    Array<PartyCharacters>Characters = new Array<PartyCharacters>();
	[Signal]
	public delegate void _ChangeCharacterEventHandler(Character character);
    public override void FillButtons(int Start,ScrollList.StartEnd StartEnd)
    {
        Characters.Clear();
        Array<PartyCharacters>Aux = GameManager.Instance.Data.Party;

        for(int i = 0;i<Aux.Count;i++){
            if(Aux[i].Active){
                Characters.Add(Aux[i]);
            }
        }
		pointerStart=Start;
        Fill(Start,StartEnd);
    }
    void Fill(int Start,ScrollList.StartEnd StartEnd){
        Buttons[0].character = Characters[Start%(Characters.Count)];
		Buttons[0].GrabFocus();
        Buttons[0].TextureNormal = (Texture2D)Buttons[0].character.Base.Icon;
		EmitSignal("_ChangeCharacter",Buttons[0].character);
	}
    public override void ClearAll()
    {
        pointerStart = 0;
        Characters.Clear();
        Buttons[0].character = null;
        
    }
}

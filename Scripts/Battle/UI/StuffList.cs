using Godot;
using System;

public partial class StuffList : VBoxContainer
{
    public int pointerStart,pointerEnd,activeButtons;
    public BattleCharacter character;
    	public virtual void FillButtons(int Start,ScrollList.StartEnd StartEnd){}
        public virtual void ClearAll(){
                    
        }
}


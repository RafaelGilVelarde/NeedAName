using Godot;
using System;

public partial class ItemTossAmountList : StuffList
{
    [Export] ItemsScreen Screen;
    [Export] public Button Amount;

    public override void FillButtons(int Start, ScrollList.StartEnd StartEnd)
    {
        if(Start>=0 && Start<=Screen.CurrentItem.Amount){
            pointerStart=Start;
            Amount.Text = pointerStart.ToString();
        }
        Amount.GrabFocus();
    }
}

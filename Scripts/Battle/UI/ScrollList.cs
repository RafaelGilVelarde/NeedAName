using Godot;
using System;
using System.Diagnostics;

public partial class ScrollList : Control
{
    public enum StartEnd{
        Start,
        End,
        Regular
    }
    public override void _Ready()
	{
		FocusEntered+=Scroll;
	}

    [Export]StartEnd startEnd;
    [Export]StuffList List;
    void Scroll(){
        switch (startEnd){
            case  StartEnd.Start:
                    List.FillButtons(List.pointerStart-1,startEnd); 
            break;
            case StartEnd.End:
                    List.FillButtons(List.pointerStart+1,startEnd);                      
            break;
            case StartEnd.Regular:
                    List.FillButtons(List.pointerStart,startEnd);
            break;
        }
    }
}

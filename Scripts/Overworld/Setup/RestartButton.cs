using Godot;
using System;

public partial class RestartButton : Button
{    	public override void _Ready()
	{
        Pressed+=ButtonWasPressed;
	}

    void ButtonWasPressed(){
        GameManager.Instance.Restart();
    }
}

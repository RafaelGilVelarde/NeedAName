using Godot;
using System;

public partial class BattleCamera : Camera2D
{
    public override void _EnterTree()
	{
	}
    public override void _Ready()
    {
        GameManager.Instance.CallDeferred("SetBattleCamera",this);
    }
}

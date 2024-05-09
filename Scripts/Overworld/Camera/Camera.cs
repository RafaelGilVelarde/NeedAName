using Godot;
using System;

public partial class Camera : Camera2D
{
    public override void _EnterTree()
	{
	}
    public override void _Ready()
    {
        if(GameManager.Instance.OverworldCam==null){
            GameManager.Instance.CallDeferred("SetCamera",this);
        }
    }
}

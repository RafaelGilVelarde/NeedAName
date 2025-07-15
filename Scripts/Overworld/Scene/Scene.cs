using Godot;
using System;

public partial class Scene : Node2D
{
    [Signal]
    public delegate void _SceneLoadedEventHandler();
    [Export] Color TransitionColor = Color.FromHsv(0,0,0,0);
    [Export] public CutscenePlayer CutsceneAnimator;

    public override void _Ready()
    {
        base._Ready();
        EmitSignal("_SceneLoaded");
        GameManager.Instance.MoveCharactersToScene(this);
        GameManager.Instance.PlayTransition(TransitionColor);
        GameManager.Instance.TransitionTween.Finished += Setup;
        YSortEnabled = true;
        //SceneTreeTimer timer = GetTree().CreateTimer(0.8f,true,true,true);
        //timer.Timeout+=()=>Setup();

    }
    public virtual void Setup(){
        GameManager.Instance.controller.SetControllable(true);
    }
}

using Godot;
using System;

public partial class Scene : Node
{
    [Signal]
    public delegate void _SceneLoadedEventHandler();
    [Export] Color TransitionColor = Color.FromHsv(0,0,0,0);

    public override void _Ready()
    {
        base._Ready();
        EmitSignal("_SceneLoaded");
        GameManager.Instance.MoveCharactersToScene();
        GameManager.Instance.PlayTransition(TransitionColor);
        GameManager.Instance.TransitionTween.Finished += Setup;
        //SceneTreeTimer timer = GetTree().CreateTimer(0.8f,true,true,true);
		//timer.Timeout+=()=>Setup();
        
    }
    public virtual void Setup(){
        GameManager.Instance.controller.SetControllable(true);
    }
}

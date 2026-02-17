using Godot;
using System;

public partial class Scene : Node2D
{
    [Signal]
    public delegate void _SceneLoadedEventHandler();
    [Export] Color TransitionColor = Color.FromHsv(0,0,0,0);
    [Export] public CutscenePlayer CutsceneAnimator;
    [Export] public int BGMIndex {get; protected set;}

    public static Scene CurrentScene;


    public override void _Ready()
    {
        CurrentScene = this;
        base._Ready();
        GameManager.Instance.MoveCharactersToScene(this);
        GameManager.Instance.PlayTransition(TransitionColor);
        GameManager.Instance.TransitionTween.Finished += Setup;
        YSortEnabled = true;
        EmitSignal("_SceneLoaded");

        //SceneTreeTimer timer = GetTree().CreateTimer(0.8f,true,true,true);
        //timer.Timeout+=()=>Setup();

    }
    public virtual void Setup()
    {
        GameManager Game = GameManager.Instance;
        Game.controller.SetControllable(true);
        Game.OverworldCam.PositionSmoothingEnabled = true;
        Game.OverworldCam.CameraParent = GetNode("./Cams");	
        Game.BattleCam.CameraParent = GetNode("./Cams");			
        GameManager.Instance.PlayAudio(BGMIndex);
    }
}

using Godot;
using System;

public partial class Scene : Node
{
    [Signal]
    public delegate void _SceneLoadedEventHandler();

    public override void _Ready()
    {
        base._Ready();
        EmitSignal("_SceneLoaded");
        GameManager.Instance.MoveCharactersToScene();
    }
}

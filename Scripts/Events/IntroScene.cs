using Godot;
using System;

public partial class IntroScene : Node
{
    [Export] string IntroDialogue;
    Callable StartGame;
    DialogicCSharp Dialogue;
    public override void _Ready()
    {
        SceneTreeTimer timer = GetTree().CreateTimer(0.5f);
        timer.Timeout += StartScene;
        base._Ready();
    }
    void StartScene(){
        StartGame = new Callable(this,MethodName.GoToFirstMap);
        Dialogue = DialogicCSharp.instance;
        Dialogue.StartDialogue(IntroDialogue,false,false);
        Dialogue.DialogicRoot.Connect("signal_event",StartGame);
    }
    void GoToFirstMap(string argument){
        Dialogue.DialogicRoot.Disconnect("signal_event",StartGame);
        GameManager.Instance.LoadFirstScene();
    }

}

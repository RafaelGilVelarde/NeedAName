using Godot;
using System;

public partial class IntroScene : Node
{
    [Export] string IntroDialogue;
    Callable StartGame;
    [Export] int AudioIndex;
    DialogicCSharp Dialogue;
    [Export] Color TransitionColor = Color.Color8(0,0,0,0);
    public override void _Ready()
    {
        GameManager.Instance.PlayTransition(TransitionColor);
        GameManager.Instance.TransitionTween.Finished+=StartScene;
        GameManager.Instance.CurrentSaveExists = false;
        //SceneTreeTimer timer = GetTree().CreateTimer(0.5f);
        //timer.Timeout += StartScene;
        base._Ready();
    }
    void StartScene(){
        GameManager.Instance.PlayAudio(AudioIndex);
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

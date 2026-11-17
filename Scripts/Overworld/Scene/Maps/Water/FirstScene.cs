using Godot;
using System;

public partial class FirstScene : Scene
{
    [Export] string Timeline, KeyFlag;
    Callable EndDialogueCall;
    public override void Setup()
    {
        if(!(bool)GameManager.Instance.Data.Flags.Dict[KeyFlag]){
            GameManager.Instance.StopAudio();
            EndDialogueCall = new Callable(this,MethodName.EndDialogue);
            DialogicCSharp DialogicInstance = DialogicCSharp.instance;
            DialogicInstance.StartDialogue(Timeline,true,false);
            DialogicInstance.DialogicRoot.Connect("timeline_ended",EndDialogueCall);
        }
        else{
            base.Setup();
        }
    }
    void EndDialogue(){
        DialogicCSharp DialogicInstance = DialogicCSharp.instance;
        DialogicInstance.DialogicRoot.Disconnect("timeline_ended",EndDialogueCall);
        GameManager.Instance.Data.Flags.Dict[KeyFlag] = true;
        GameManager.Instance.PlayAudio(BGMIndex);
        GameManager.Instance.controller.SetControllable(true);
        GameManager.Instance.controller.EnableHurtbox(true);
    }
}

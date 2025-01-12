using Godot;
using System;

public partial class FirstScene : Scene
{
    [Export] string Timeline;
    Callable EndDialogueCall;
    public override void Setup()
    {
        if(!GameManager.Instance.Data.Flags.EventFlags[0]){
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
        GameManager.Instance.Data.Flags.EventFlags[0] = true;
        GameManager.Instance.controller.SetControllable(true);
    }
}

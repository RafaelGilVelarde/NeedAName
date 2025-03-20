using Godot;
using System;

public partial class FirstScene : Scene
{
    [Export] string Timeline;
    [Export] int EventFlag;
    Callable EndDialogueCall;
    public override void Setup()
    {
        if(!GameManager.Instance.Data.Flags.EventFlags[EventFlag]){
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
        GameManager.Instance.Data.Flags.EventFlags[EventFlag] = true;
        GameManager.Instance.controller.SetControllable(true);
    }
}

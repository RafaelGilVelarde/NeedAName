using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class Interact : CollisionShape2D
{

    [Export]String Timeline="Test";
    [Export]bool PauseWhenDialogue;

    Callable disable;
    public override void _Ready()
    {
    }


    public virtual void interact(){
        Array<OverworldController> party=GameManager.Instance.Characters;
        for(int i=0;i<party.Count;i++){
            if(party[i].Leader){
                party[i].InteractCollider.GetChild<CollisionShape2D>(0).Disabled=true;
                Debug.WriteLine("Hello");
            }
        }
        disable=new Callable(this,MethodName.Disable);
        DialogicCSharp DialogicInstance= (DialogicCSharp)DialogicCSharp.instance;
        DialogicInstance.StartDialogue(Timeline,PauseWhenDialogue,false);
        DialogicInstance.DialogicRoot.Connect("timeline_ended",disable);
    }
    public virtual void Disable(){
        Array<OverworldController> party=GameManager.Instance.Characters;
        for(int i=0;i<party.Count;i++){
            if(party[i].Leader){
                party[i].InteractCollider.GetChild<CollisionShape2D>(0).Disabled=false;
            }
        }
        DialogicCSharp DialogicInstance= (DialogicCSharp)DialogicCSharp.instance;
        DialogicInstance.Disconnect("timeline_ended",disable);
    }
}

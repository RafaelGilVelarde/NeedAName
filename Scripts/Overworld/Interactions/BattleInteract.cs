using Godot;
using System;
using System.Diagnostics;


public partial class BattleInteract : Interact
{
    [Export] BattleStart battleStart;
        Callable startBattle;
        Callable EndTimeline;
    public override void interact()
    {
        Debug.WriteLine("StartDialogue");
        startBattle=new Callable(this,MethodName.Battle);
        EndTimeline=new Callable(this,MethodName.EndDialogue);
        
        Node DialogicRoot=DialogicCSharp.instance.DialogicRoot;
        DialogicRoot.Connect("signal_event",startBattle);
        DialogicRoot.Connect("timeline_ended",EndTimeline);
        base.interact();
    }
    void Battle(string argument){
        Node DialogicRoot=DialogicCSharp.instance.DialogicRoot;
        if (argument == "Fight"){
            battleStart.StartBattle();
            DialogicRoot.Disconnect("signal_event",startBattle);
        }        
    }
    void EndDialogue(){
        Node DialogicRoot=DialogicCSharp.instance.DialogicRoot;
        DialogicRoot.Disconnect("signal_event",startBattle);
        DialogicRoot.Disconnect("timeline_ended",EndTimeline);
    }
}

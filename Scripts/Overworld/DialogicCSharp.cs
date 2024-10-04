using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

public partial class DialogicCSharp : Node
{
    public static DialogicCSharp instance;
    public Node DialogicRoot;
    Node Styles;
    bool Paused, autoAdvance;
        Callable Check;


    Array<InputEvent> EventAux=new Array<InputEvent>();
    Callable endAutoAdvance;
    public override void _EnterTree()
    {

    }
    public override void _Ready()
    {
        Callable.From(Setup).CallDeferred();	
    }
    public void StartDialogue(string Timeline, bool Pause, bool auto){

        if(auto){
            autoAdvance=true;
        }

        DialogicRoot.Call("start",Timeline);
        Check=new Callable(this,MethodName.check);
        DialogicRoot.Connect("signal_event",Check);

        Node aux= (Node)Styles.Call("get_layout_node");
        aux.ProcessMode=ProcessModeEnum.Always;
        if(Pause){
            Array<OverworldController> party=GameManager.Instance.Characters;
            for(int i=0;i<party.Count;i++){
                if(party[i].Leader){
                    party[i].EnterExitDialogue(Pause);
                }
            }          
        }
    }
    public void AutoAdvance(bool On,bool UntilNextInput){
        Node Inputs=GetNode("/root/Dialogic/Inputs");
        RefCounted AutoAdvance= (RefCounted)Inputs.Get("auto_advance");
        RefCounted ManualAdvance= (RefCounted)Inputs.Get("manual_advance");
        if(On){
            if(UntilNextInput){
                AutoAdvance.Set("enabled_until_next_event",true);
            }
            else{
                AutoAdvance.Set("enabled_forced",true);
                ManualAdvance.Set("enabled_forced",false);
                AutoAdvance.Set("enabled_until_next_event",false);
                DialogicRoot.Connect("signal_event",endAutoAdvance);
            }
        }
        else{
            AutoAdvance.Set("enabled_forced",false);
            autoAdvance=false;
            if(DialogicRoot.IsConnected("signal_event",endAutoAdvance)){
                DialogicRoot.Disconnect("signal_event",endAutoAdvance);
            }
        }
    }
    void EndAutoAdvance(string argument){
        if(argument=="EndAuto"){
            AutoAdvance(false,false);
        }
    }
    void UnPause(){
        Resource CurrentTimeline = (Resource)DialogicRoot.Get("current_timeline");
        Debug.WriteLine("timeline UnPause: "+CurrentTimeline);
        AutoAdvance(false,false);
        Array<OverworldController> party=GameManager.Instance.Characters;
        for(int i=0;i<party.Count;i++){
            if(party[i].Leader){
                party[i].EnterExitDialogue(false);
            }
        }    
        DialogicRoot.Disconnect("signal_event",Check);
    }
    void ManualAdvanceOff(){
        if(autoAdvance){
            Node Inputs=GetNode("/root/Dialogic/Inputs");
            Debug.WriteLine("Manual: "+Inputs.Call("is_manualadvance_enabled"));
            Inputs.Call("set_manualadvance",false,false);
            Debug.WriteLine("Manual: "+Inputs.Call("is_manualadvance_enabled"));
        }
    }
    private async void Setup()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        instance=this;
        DialogicRoot = GetNode("/root/Dialogic");
        DialogicRoot.ProcessMode=ProcessModeEnum.Always;
        Callable unPause = new Callable(this, MethodName.UnPause);
        endAutoAdvance = new Callable(this, MethodName.EndAutoAdvance);
        DialogicRoot.Connect("timeline_ended",unPause);
        Styles=GetNode("/root/Dialogic/Styles");
        
    }
    void check(string argument){
        Resource CurrentTimeline = (Resource)DialogicRoot.Get("current_timeline");
        Debug.WriteLine("timeline: "+CurrentTimeline);
        if(CurrentTimeline==null){
            Debug.WriteLine("null");
        }
    }
}

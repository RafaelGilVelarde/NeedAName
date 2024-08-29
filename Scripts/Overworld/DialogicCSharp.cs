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


    Array<InputEvent> EventAux=new Array<InputEvent>();
    Callable endAutoAdvance;
    public override void _EnterTree()
    {

        /*Callable manualOff = new Callable(this, MethodName.ManualAdvanceOff);
        DialogicRoot.Connect("timeline_started",manualOff);*/
    }
    public override void _Ready()
    {
        Callable.From(Setup).CallDeferred();	
    }
    public void StartDialogue(String Timeline, bool Pause, bool auto){
        if(auto){
            autoAdvance=true;
        }
        DialogicRoot.Call("start",Timeline);
        Debug.WriteLine(Styles);
        Node aux= (Node)Styles.Call("get_layout_node");
        aux.ProcessMode=ProcessModeEnum.Always;
        if(Pause){
            GetTree().Paused=true;            
        }

        Node Inputs=GetNode("/root/Dialogic/Inputs");
        /*RefCounted AutoAdvance= (RefCounted)Inputs.Get("auto_advance");
        RefCounted ManualAdvance= (RefCounted)Inputs.Get("manual_advance");
        AutoAdvance.Set("enabled_forced",auto);
        ManualAdvance.Set("enabled_forced",!auto);       
        Debug.WriteLine("Manual: ",ManualAdvance.Get("enabled_forced"));*/
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
                //EventAux.Clear();
                /*Debug.WriteLine("Manual: "+Inputs.Call("is_manualadvance_enabled"));
                Inputs.Call("set_manualadvance",false,false);
                Debug.WriteLine("Manual: "+Inputs.Call("is_manualadvance_enabled"));
                //ProjectSettings.SetSetting("dialogic/text/input_action","");*/
                AutoAdvance.Set("enabled_forced",true);
                ManualAdvance.Set("enabled_forced",false);
                Debug.WriteLine("Manual: "+ManualAdvance.Call("is_enabled"));
                AutoAdvance.Set("enabled_until_next_event",false);
                DialogicRoot.Connect("signal_event",endAutoAdvance);
            }
        }
        else{
            Debug.WriteLine("Manual: "+ManualAdvance.Call("is_enabled"));
            AutoAdvance.Set("enabled_forced",false);
            //ManualAdvance.Set("enabled_forced",true);
            //Debug.WriteLine("Manual: "+ManualAdvance.Get("enabled_forced"));
            autoAdvance=false;
            //ProjectSettings.SetSetting("dialogic/text/input_action","dialogic_default_action");
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
                AutoAdvance(false,false);
                GetTree().Paused=false;
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
}

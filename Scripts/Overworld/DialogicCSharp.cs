using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;

public partial class DialogicCSharp : Node
{
    public static DialogicCSharp instance;
    public Node DialogicRoot;
    Node Styles;
    [Export] bool Paused, autoAdvance;
    Callable Check;
    [Export] Array<string> DialogueStyles, Timelines = new Array<string>();


    Array<InputEvent> EventAux = new Array<InputEvent>();
    Callable endAutoAdvance;

    [Signal]
    public delegate void _NextTimelineEventHandler(string Timeline);


    public override void _Ready()
    {
        Callable.From(Setup).CallDeferred();
    }
    public async Task StartDialogue(string Timeline, bool Pause, bool auto)
    {
        if (!Timelines.Contains(Timeline))
        {
            Timelines.Add(Timeline);
            
        }
        if (Timelines.Count == 1)
        {
            Start();
        }
        else
        {
            string Aux = (await ToSignal(this, "_NextTimeline"))[0].ToString();
            if (Aux == Timelines[0])
            {
                Start();
            }          
        }

        void Start()
        {
            if (auto)
            {
                autoAdvance = true;
            }
            Debug.WriteLine("Starting Timeline: "+Timelines[0]);
            DialogicRoot.Call("start", Timelines[0]);
            Debug.WriteLine("TimelineStarted, Root:"+DialogicRoot);
            //Node aux= (Node)Styles.Call("get_layout_node");
            //aux.ProcessMode=ProcessModeEnum.Always;
            if (Pause)
            {
                Array<PlayerController> party = GameManager.Instance.Characters;
                for (int i = 0; i < party.Count; i++)
                {
                    if (party[i].Leader)
                    {
                        party[i].CallDeferred("EnterExitDialogue", Pause);
                    }
                }
            }            
        }
    }
    public void AutoAdvance(bool On, bool UntilNextInput)
    {
        Node Inputs = GetNode("/root/Dialogic/Inputs");
        RefCounted AutoAdvance = (RefCounted)Inputs.Get("auto_advance");
        RefCounted ManualAdvance = (RefCounted)Inputs.Get("manual_advance");
        if (On)
        {
            if (UntilNextInput)
            {
                AutoAdvance.Set("enabled_until_next_event", true);
            }
            else
            {
                AutoAdvance.Set("enabled_forced", true);
                ManualAdvance.Set("enabled_forced", false);
                AutoAdvance.Set("enabled_until_next_event", false);
                DialogicRoot.Connect("signal_event", endAutoAdvance);
            }
        }
        else
        {
            AutoAdvance.Set("enabled_forced", false);
            autoAdvance = false;
            if (DialogicRoot.IsConnected("signal_event", endAutoAdvance))
            {
                DialogicRoot.Disconnect("signal_event", endAutoAdvance);
            }
        }
    }
    void EndAutoAdvance(string argument)
    {
        if (argument == "EndAuto")
        {
            AutoAdvance(false, false);
        }
    }
    void UnPause()
    {
        Timelines.RemoveAt(0);
        if (Timelines.Count > 0)
        {
            EmitSignal("_NextTimeline", Timelines[0]);
        }
        else
        {
            Resource CurrentTimeline = (Resource)DialogicRoot.Get("current_timeline");
            AutoAdvance(false, false);
            Array<PlayerController> party = GameManager.Instance.Characters;
            for (int i = 0; i < party.Count; i++)
            {
                if (party[i].Leader)
                {
                    party[i].EnterExitDialogue(false);
                }
            }            
        }
    }
    void ManualAdvanceOff()
    {
        if (autoAdvance)
        {
            Node Inputs = GetNode("/root/Dialogic/Inputs");
            Inputs.Call("set_manualadvance", false, false);
        }
    }
    private async void Setup()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        instance = this;
        DialogicRoot = GetNode("/root/Dialogic");
        DialogicRoot.ProcessMode = ProcessModeEnum.Always;
        Callable unPause = new Callable(this, MethodName.UnPause);
        endAutoAdvance = new Callable(this, MethodName.EndAutoAdvance);
        DialogicRoot.Connect("timeline_ended", unPause);
        Styles = GetNode("/root/Dialogic/Styles");
        for (int i = 0; i < DialogueStyles.Count; i++)
        {
            Resource aux = ResourceLoader.Load(DialogueStyles[i]);
            aux.Call("prepare");
        }

    }
    public void SetVariable(string VariableName, string Folder, Variant Value)
    {
        Node Var = (Node)DialogicRoot.Get("VAR");
        GodotObject Aux = (GodotObject)Var.Get(Folder);
        Aux.Set(VariableName, Value);
    }
    public string GetVariable(string VariableName,string Folder){
        Node Var = (Node)DialogicRoot.Get("VAR");
        GodotObject Aux = (GodotObject)Var.Get(Folder);
        return (string)Aux.Get(VariableName);
    }
}

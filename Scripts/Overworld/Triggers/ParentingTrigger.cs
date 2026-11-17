using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class ParentingTrigger : Area2D
{
    [Export]Node2D Main;

    public override void _Ready()
    {
        base._Ready();
        if (Main == null)
        {
            Main = GetParent<Node2D>();
        }
        AreaEntered+=OnTriggerEnter;
        AreaExited+=OnTriggerExit;
        
    }
    void OnTriggerEnter(Area2D node)
    {
        if (node.IsInGroup("PlayerOverworldHurtbox"))
        {
            OverworldController Aux = node.GetParent<OverworldController>();
            if (Aux.Grandparent != Main)
            {
            Debug.WriteLine("Controllerentered: "+node.Name);
                SetDeferred("monitoring", false);
                Aux.SetGrandparent(Main);
                Aux.GrandParentNode();                            
                SceneTreeTimer timer = GetTree().CreateTimer(0.1,true,true);
                timer.Timeout+=()=>{Monitoring = true;};
            }
        }
    }
    void OnTriggerExit(Area2D node)
    {
        if (Monitoring)
        {
            if (node.IsInGroup("PlayerOverworldHurtbox"))
            {
                Debug.WriteLine("ExitingArea");
                OverworldController Aux = node.GetParent<OverworldController>();
                if(Aux.Grandparent == Main)
                {
                    SetDeferred("monitoring", false);
                    Aux.SetGrandparent();         
                    Aux.GrandParentNode();   
                    SceneTreeTimer timer = GetTree().CreateTimer(0.1,true,true);
                    timer.Timeout+=()=>{Monitoring = true;};
                }            
            }
            
        }
    }
}

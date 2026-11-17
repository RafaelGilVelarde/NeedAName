using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;

public partial class CheckFlagNode : Node
{
    [Export] Array<CheckFlags> CheckFlags;
    [Export] public Node MainNode;

    [Export] Array<CheckQuests> checkQuests = new Array<CheckQuests>();
    [Export] Array<CheckFlagEffects> CheckEffects = new Array<CheckFlagEffects>();

    public override void _Ready()
    {
       ((Scene)GetTree().CurrentScene)._SceneLoaded+=Activate;
    }
    void Activate()
    {
        int aux = 0;
        for(int i = 0; i < CheckFlags.Count; i++)
        {
            if (CheckFlags[i].Check())
            {
                aux++;
            }
        }
        for(int i = 0; i < checkQuests.Count; i++)
        {
            if (checkQuests[i].Check())
            {
                aux++;
            }
        }

        if(aux == CheckFlags.Count+checkQuests.Count)
        {
            for(int i = 0; i < CheckEffects.Count; i++)
            {
                CheckEffects[i].Effect(this);
            }
        }    
       ((Scene)GetTree().CurrentScene)._SceneLoaded-=Activate;
    }
}

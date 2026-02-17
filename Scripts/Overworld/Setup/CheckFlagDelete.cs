using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;

public partial class CheckFlagDelete : Node
{
    [Export] Array<CheckFlags> CheckFlags;
    [Export] Node MainNode;

    public override void _Ready()
    {
       ((Scene)GetTree().CurrentScene)._SceneLoaded+=Delete;
    }
    void Delete()
    {
        int aux = 0;
        for(int i = 0; i < CheckFlags.Count; i++)
        {
            if (CheckFlags[i].Check())
            {
                aux++;
            }
        }
        if(aux == CheckFlags.Count)
        {
            MainNode.QueueFree();
            QueueFree();
        }    
       ((Scene)GetTree().CurrentScene)._SceneLoaded-=Delete;
    }
}

using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class CheckValueInteract : InteractText
{
    [Export] Array<CheckValueTimeline> CheckValues;
    public override void interact(OverworldController Player)
    {
        int Group = TimelineGroupIndex;
        for(int i = 0;i<CheckValues.Count;i++){
            TimelineGroupIndex = CheckValues[i].GetTimeline(TimelineGroupIndex);

        }

        if(Group!=TimelineGroupIndex){
            SpokenTo = false;
            TimelineIndex = 0;
        }
        base.interact(Player);
    }

}

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
        Timelines timeline = TimelineGroup[TimelineGroupIndex];
        Array<FlagType> flagTypes = timeline.flagTypes;
        Flags flags = GameManager.Instance.Data.Flags;
        for(int i =0;i<flagTypes.Count;i++){
            switch (flagTypes[i]){
                case FlagType.Puzzle:
                    flags.ChangeBoolFlag(timeline.FlagIndexes[i],true,FlagType.Puzzle);
                break;
                case FlagType.Event:
                    flags.ChangeBoolFlag(timeline.FlagIndexes[i],true,FlagType.Event);
                break;
                case FlagType.Item:
                    flags.ChangeBoolFlag(timeline.FlagIndexes[i],true,FlagType.Item);
                break;
                case FlagType.Dialogue:
                    flags.ChangeBoolFlag(timeline.FlagIndexes[i],true,FlagType.Dialogue);
                                Debug.WriteLine("Dialogue: "+timeline.FlagIndexes[i]);
                break;
            }
            Debug.WriteLine("AAAA");
        }
        if(Group!=TimelineGroupIndex){
            SpokenTo = false;
            TimelineIndex = 0;
        }
        base.interact(Player);
    }

}

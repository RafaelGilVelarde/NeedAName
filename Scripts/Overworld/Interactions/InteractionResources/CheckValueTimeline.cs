using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class CheckValueTimeline : Resource
{
    [Export] int TimelineIndex; 
    [Export] Array<int> FlagIndexes;
    [Export] Array<FlagType> flagTypes;

    public int GetTimeline(int Index){
        int aux = 0;
        Flags flags = GameManager.Instance.Data.Flags;
        Array<int> DialogueIndex = new Array<int>();
        for(int i =0;i<FlagIndexes.Count;i++){
            int IndexAux = FlagIndexes[i];
            switch(flagTypes[i]){
                case FlagType.Puzzle:
                    if(flags.PuzzleFlags[IndexAux]){
                        aux++;
                    }
                break;
                case FlagType.Event:
                    if(flags.EventFlags[IndexAux]){
                        aux++;
                    }
                break;
                case FlagType.Item:
                     if(flags.ItemGiven[IndexAux]){
                        aux++;
                    }
                break;
                case FlagType.Dialogue:
                    if(flags.DialogueFlags[IndexAux]){
                        aux++;
                    }
                break;
            }
        }
        Debug.WriteLine("CheckTimeline: "+this +": "+ aux);
        if (aux >= FlagIndexes.Count)
        {
            return TimelineIndex;
        }
        else
        {
            return Index;
        }
    }
    
}

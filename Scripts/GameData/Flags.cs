using Godot;
using Godot.Collections;
using System;

public enum FlagType{
    Puzzle, 
    Event,
    Item,
    Dialogue
}
[GlobalClass]
public partial class Flags : Resource
{
    [Export] public Array<bool> ItemGiven;
    [Export] public Array<bool> DialogueFlags;
    [Export] public Array<bool> PuzzleFlags;
    [Export] public Array<bool> EventFlags;
    [Export] public Array<int> PuzzleIntFlags; 
    [Export] public Array<int> EventIntFlags; 

    [Signal]
    public delegate void _PuzzleFlagsBoolChangedEventHandler(int Change,bool Changed);
    [Signal]
    public delegate void _PuzzleFlagsIntChangedEventHandler(int Change, bool Changed);
    [Signal]
    public delegate void _EventFlagsBoolChangedEventHandler(int Change, bool Changed);
    [Signal]
    public delegate void _EventFlagsIntChangedEventHandler(int Change, bool Changed);
    [Signal]
    public delegate void _ItemFlagsBoolChangedEventHandler(int Change, bool Changed);
    [Signal]
    public delegate void _DialogueFlagsBoolChangedEventHandler(int Change, bool Changed);

    public void ChangeBoolFlag(int index, bool change, FlagType type){
        bool changed = false;
        switch (type){
            case FlagType.Puzzle:
                changed = !PuzzleFlags[index]==change;
                PuzzleFlags[index] = change;
                EmitSignal("_PuzzleFlagsBoolChanged",index, changed);
            break;
            case FlagType.Event:
                changed = !EventFlags[index]==change;
                EventFlags[index] = change;
                EmitSignal("_EventFlagsBoolChanged",index, changed);
            break;
            case FlagType.Item:
                changed = !ItemGiven[index]==change;
                ItemGiven[index] = change;
                EmitSignal("_ItemFlagsBoolChanged",index, changed);
            break;
            case FlagType.Dialogue:
                changed = !DialogueFlags[index]==change;
                DialogueFlags[index] = change;
                EmitSignal("_DialogueFlagsBoolChanged",index, changed);
            break;
        }
    }
        public void ChangeIntFlag(int index, int change, FlagType type){
        bool changed = false;
        switch (type){
            case FlagType.Puzzle:
                changed = !(PuzzleIntFlags[index]==change);
                PuzzleIntFlags[index] = change;
                EmitSignal("_PuzzleFlagsIntChanged",index, changed);
            break;
            case FlagType.Event:
                changed = !(EventIntFlags[index]==change);
                EventIntFlags[index] = change;
                EmitSignal("_EventFlagsIntChanged",index,changed);
            break;
        }
    }



}

using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

[GlobalClass]
public partial class CheckFlags : Resource
{
    enum FlagValueType
    {
        Int,
        Bool,
    }
    [Export] FlagType flagType;
    [Export] int index, intResult;
    [Export] bool boolResult;
    [Export] FlagValueType ValueType;

    public bool Check()
    {
        Flags flags = GameManager.Instance.Data.Flags;
        switch (flagType)
        {
            case FlagType.Puzzle:
                if(ValueType == FlagValueType.Bool)
                {
                    return flags.PuzzleFlags[index] == boolResult;
                }
                else
                {
                    return flags.PuzzleIntFlags[index] == intResult;        
                }
            case FlagType.Event:
                if(ValueType == FlagValueType.Bool)
                {
                    return flags.EventFlags[index] == boolResult;
                }
                else
                {
                    return flags.EventIntFlags[index] == intResult;        
                }
            case FlagType.Item:
                return flags.ItemGiven[index] == boolResult;
            case FlagType.Dialogue:
                return flags.DialogueFlags[index] == boolResult;
        }
        return false;
    }
}

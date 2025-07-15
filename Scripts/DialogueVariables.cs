using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class DialogueVariables : Resource
{
    [Export] public FlagType Flagtype;
    [Export] public int index;
    [Export] public bool BoolFlag;
    [Export] public string Category, VariableName;

    public Array<bool> ArrayBool()
    {
        Flags flags = GameManager.Instance.Data.Flags;
        switch (Flagtype)
        {
            case FlagType.Puzzle:
                Category = "Puzzle";
                return flags.PuzzleFlags;
            case FlagType.Event:
                Category = "Event";
                return flags.EventFlags;
            case FlagType.Item:
                Category = "ItemGiven";
                return flags.ItemGiven;
            case FlagType.Dialogue:
                Category = "Dialogue";
                return flags.DialogueFlags;
        }
        return null;
    }
    public Array<int> ArrayInt()
    {
        Flags flags = GameManager.Instance.Data.Flags;
        switch (Flagtype)
        {
            case FlagType.Puzzle:
                Category = "PuzzleInt";
                return flags.PuzzleIntFlags;
            case FlagType.Event:
                Category = "EventInt";
                return flags.EventIntFlags;
        }
        return null;
    }
    public void SetVariable()
    {
        DialogicCSharp Dialogic = DialogicCSharp.instance;
        if (BoolFlag)
        {
            Dialogic.SetVariable(VariableName, Category, ArrayBool()[index]);
        }
        else
        {
            Dialogic.SetVariable(VariableName, Category, ArrayInt()[index]);
        }
    }
}



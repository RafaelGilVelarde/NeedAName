using Godot;
using System;

public partial class PuzzleCheck : Node
{
    [Export] protected bool HasDialogue = true, ChecksBool, ChecksInt;
    [Export] public int FlagIndex;

    public override void _EnterTree()
    {
        Flags Aux = GameManager.Instance.Data.Flags;
        if(ChecksBool){
            Aux._PuzzleFlagsBoolChanged+=Check;
        }
        if(ChecksInt){
            Aux._PuzzleFlagsIntChanged+=CheckInt;
        }
    }
    public override void _ExitTree()
    {
        Flags Aux = GameManager.Instance.Data.Flags;
        if(ChecksBool){
            Aux._PuzzleFlagsBoolChanged-=Check;
        }
        if(ChecksInt){
            Aux._PuzzleFlagsIntChanged-=CheckInt;
        }
    }
    public virtual void Check(int Index, bool Changed){

    }
    public virtual void CheckInt(int Index, bool Changed){

    }
    public virtual void ActivateEffect(){

    }

}

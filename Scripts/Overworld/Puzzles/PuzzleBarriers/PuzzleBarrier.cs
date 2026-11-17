using Godot;
using System;
using System.Diagnostics;
using System.Reflection;

public partial class PuzzleCheck : Node
{
    [Export] protected bool HasDialogue = true;
    [Export] public string KeyIndex;

    public override void _EnterTree()
    {
        Flags Aux = GameManager.Instance.Data.Flags;
        Aux._DictFlagsChanged+=CheckDict;
        /*if(ChecksBool){
            Aux._PuzzleFlagsBoolChanged+=Check;
        }
        if(ChecksInt){
            Aux._PuzzleFlagsIntChanged+=CheckInt;
        }*/
    }
    public override void _ExitTree()
    {
        Flags Aux = GameManager.Instance.Data.Flags;
        /*if(ChecksBool){
            Aux._PuzzleFlagsBoolChanged-=Check;
        }
        if(ChecksInt){
            Aux._PuzzleFlagsIntChanged-=CheckInt;
        }*/
        Aux._DictFlagsChanged-=CheckDict;
    }

    public virtual void CheckDict(string Key, bool Changed){
        Debug.WriteLine("Checkng dict");
    }
    public virtual void ActivateEffect(){

    }

}

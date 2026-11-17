using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class DialogueVariables : Resource
{
    [Export] public string Category, VariableName, DictKey;


    public void SetVariable()
    {
        DialogicCSharp Dialogic = DialogicCSharp.instance;

            Dialogic.SetVariable(VariableName, Category, GameManager.Instance.Data.Flags.Dict[DictKey]);
    }
}



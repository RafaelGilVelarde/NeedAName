using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

[GlobalClass]
public partial class CheckFlags : Resource
{

    [Export] string KeyIndex;
    [Export] Variant Result;

    public bool Check()
    {
        Flags flags = GameManager.Instance.Data.Flags;
        return flags.Dict[KeyIndex].Equals(Result);
    }
}

using Godot;
using Godot.Collections;
using System;

public partial class SetVariableManuallyInteractEffect : InteractEffect
{
    [Export] Dictionary<string,Variant> ManualChange;
    public override void ConnectCall()
    {
        base.ConnectCall();
        StartInteract = new Callable(this,MethodName.ManualFlagUpdate);

    }
    protected virtual void ManualFlagUpdate(string argument)
    {
        Flags flags = GameManager.Instance.Data.Flags;
        string[] Aux = argument.Split("_");
        if(Aux[0] == "Change")
        {
            flags.ChangeDictFlag(Aux[1],ManualChange[Aux[1]]);
        }
    }
}

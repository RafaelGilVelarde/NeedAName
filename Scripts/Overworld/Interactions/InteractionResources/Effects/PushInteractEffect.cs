using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;

public partial class PushInteractEffect : InteractEffect
{
    [Export] Array<Vector2> PushOffset;
    [Export] Array<float> PushTimes;

    public override void ConnectCall()
    {
        base.ConnectCall();
        StartInteract = new Callable(this,MethodName.Push);

    }
    protected virtual void Push(string argument)
    {
        string Aux = "";
        string Aux2 = "";
        if (argument.Contains("_"))
        {
            Aux = argument.Split("_")[0];
            Aux2 = argument.Split("_")[1];
        }
        if (Aux == "Push")
        {
            int Index = Aux2.ToInt();
            Tween tween = CreateTween();
            CharacterBody2D Chara = GameManager.Instance.controller.Parent;
            tween.TweenProperty(Chara, "position", Chara.GlobalPosition + PushOffset[Index],PushTimes[Index]);
        }
    }
}

using Godot;
using System;

public partial class HealInteract : InteractText
{
        Callable DoAction;
        Node DialogicRoot;
    public override void _Ready()
    {
        base._Ready();
    }   public override void interact(OverworldController Player)
    {
        DialogicRoot=DialogicCSharp.instance.DialogicRoot;
        DoAction = new Callable(this,MethodName.Action);
        DialogicRoot.Connect("signal_event",DoAction);
        base.interact(Player);
    }
    public void Action(string argument){
        if(argument=="Heal"){
            DataManager Data = GameManager.Instance.Data;
            for(int i=0;i<Data.Party.Count;i++){
                if(Data.Party[i].Active){
                    Data.Party[i].ChangeHP(Data.Party[i].TotalStats.MaxHP);
                }
            }
        }
    }
    public override void Disable()
    {
        DialogicRoot.Disconnect("signal_event",DoAction);
        base.Disable();
    }
}

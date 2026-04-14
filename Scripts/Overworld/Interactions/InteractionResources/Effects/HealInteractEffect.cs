using Godot;
using System;

public partial class HealInteractEffect : InteractEffect
{
    public override void ConnectCall()
    {
        StartInteract = new Callable(this,MethodName.Action);
        base.ConnectCall();
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

}

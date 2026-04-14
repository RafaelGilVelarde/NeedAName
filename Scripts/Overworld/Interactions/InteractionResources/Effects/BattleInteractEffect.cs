using Godot;
using System;

public partial class BattleInteractEffect : InteractEffect
{
        [Export] BattleStart battleStart;
    public override void ConnectCall()
    {
        StartInteract = new Callable(this,MethodName.Battle);
        base.ConnectCall();
    }
     void Battle(string argument){
        if (argument == "Fight"){
            battleStart.StartBattle();
        }        
    }
}

using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class BattleInteractEffectResource : InteractEffectResource
{
    [Export] Array<string> BattleStartPaths;
    Array<BattleStart> Battles = new Array<BattleStart>();
    public override void Ready()
    {
        base.Ready();
        for(int i = 0; i < BattleStartPaths.Count; i++)
        {
            Debug.WriteLine("Battlenode count: "+BattleStartPaths.Count);
            Debug.WriteLine("BattleNode: "+BaseNode.GetNode(BattleStartPaths[i]));
            Battles.Add((BattleStart)BaseNode.GetNode(BattleStartPaths[i]));
        }
    }
    public override void ConnectCall()
    {
        StartInteract = new Callable(this,MethodName.Battle);
        base.ConnectCall();
    }
     void Battle(string argument){
        string[] BattleSplit = argument.Split("_");
        if(BattleSplit[0] == "Fight")
        {
            if (BattleSplit.Length > 1)
            {
                Battles[BattleSplit[1].ToInt()].StartBattle();
            }
            else
            {
                Battles[0].StartBattle();            
            }            
        }
    }
}

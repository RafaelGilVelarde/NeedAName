using Godot;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class ChangeFlagSimple : BattleScene
{
    [Export] int FlagIndex, IntFlagValue;
    [Export] bool BoolFlagValue;
    [Export] SwitchType switchType;
    [Export] FlagType flagType;
    public override void ReturnToOverworld(){
        
        base.ReturnToOverworld();
        BattleManager Battle=BattleManager.instance;
        BattleState State=Battle.State;
        switch (State){
            case BattleState.Win:
                Debug.WriteLine("Changing Flag");
                Flags Aux = GameManager.Instance.Data.Flags;
                switch (switchType){
                    case SwitchType.Bool:
                        switch(flagType){
                            case FlagType.Puzzle:
                                Aux.ChangeBoolFlag(FlagIndex,BoolFlagValue,FlagType.Puzzle);
                            break;
                            case FlagType.Event:
                                Aux.ChangeBoolFlag(FlagIndex,BoolFlagValue,FlagType.Event);
                            break;
                        }
                    break;
                    case SwitchType.Int:
                        switch(flagType){
                            case FlagType.Puzzle:
                                Aux.ChangeIntFlag(FlagIndex,IntFlagValue,FlagType.Puzzle);
                            break;
                            case FlagType.Event:
                                Aux.ChangeIntFlag(FlagIndex,IntFlagValue,FlagType.Event);
                            break;
                        }
                    break;
                }
                //EnemyOrderPuzzle.instance.EnemyDefeated(Order);
            break;
        }
    }
}

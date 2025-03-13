using Godot;
using System;

public partial class StrengthMapBattle : BattleScene
{
    [Export] int FlagIndex;

    public override void BattleEnd()
    {
        base.BattleEnd();
        BattleState State=Battle.State;
        switch (State){
            case BattleState.Win:
                GameManager.Instance.Data.Flags.ChangeBoolFlag(FlagIndex,true,FlagType.Puzzle);
            break;
        }
    }

}

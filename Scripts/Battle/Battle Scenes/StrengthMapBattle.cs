using Godot;
using System;

public partial class StrengthMapBattle : BattleScene
{
    [Export] int FlagIndex;
    [Export] string Key;
    [Export] Variant value;

    public override void BattleEnd()
    {
        base.BattleEnd();
        BattleState State=Battle.State;
        switch (State){
            case BattleState.Win:
                GameManager.Instance.Data.Flags.ChangeDictFlag(Key,value);
            break;
        }
    }

}

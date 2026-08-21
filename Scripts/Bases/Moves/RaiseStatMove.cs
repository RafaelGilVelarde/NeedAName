using Godot;
using Godot.Collections;
using System;
using System.IO;

[GlobalClass]
public partial class RaiseStatMove : MoveBase
{
    //[Export] StatusCondition Status;
    //[Export] Array<StatusCondition> Alt;
    [Export] protected StatChange Stats;
    /*[Export] int AtkTime, DefTime,SpAtkTime,SpDefTime, SpeedTime, StatusTime, DelayTarget;
    [Export] float AtkMult,DefMult,SpAtkMult,SpDefMult,SpeedMult, HPIncrease, WPIncrease;
    [Export] protected bool Atk,Def,Spatk,Spdef,Speed, HP, WPUp, DelayUp, StatCondition, AltStat;*/
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
		SceneTreeTimer timer=Users[0].GetTree().CreateTimer(MoveTime,true,true);
        timer.Timeout+=End;

        Users[0].changeAction(BattleCharacter.ActionState.isStatus);
        Stats.RaiseStat(Targets[0]);

        void End()
        {
            BattleManager.instance.CallDeferred("EndMove");
		}
    }
    
}

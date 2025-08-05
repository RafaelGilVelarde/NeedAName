using Godot;
using Godot.Collections;
using System;
using System.IO;

[GlobalClass]
public partial class RaiseStatMove : MoveBase
{
    [Export] int AtkTime, DefTime,SpAtkTime,SpDefTime, SpeedTime;
    [Export] float AtkMult,DefMult,SpAtkMult,SpDefMult,SpeedMult, HPIncrease, WPIncrease;
    [Export] protected bool Atk,Def,Spatk,Spdef,Speed, HP, WPUp;
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
		SceneTreeTimer timer=Users[0].GetTree().CreateTimer(MoveTime,true,true);
        timer.Timeout+=End;

        Users[0].changeAction(BattleCharacter.ActionState.isStatus);
        RaiseStat(Targets[0]);

        void End()
        {
            BattleManager.instance.CallDeferred("EndMove");
		}
    }
    protected void RaiseStat(BattleCharacter Target, int[] TimerSum = null)
    {
        if (TimerSum == null)
        {
            TimerSum = new int[]{1,1,1,1,1};
        }
        if(Atk){
            Target.AddStatMultiplier(AtkMult,AtkTime*TimerSum[0],0);
        }
        if(Def){
            Target.AddStatMultiplier(DefMult,DefTime*TimerSum[1],1);
        }
        if(Spatk){
            Target.AddStatMultiplier(SpAtkMult,SpAtkTime*TimerSum[2],2);
        }
        if(Spdef){
            Target.AddStatMultiplier(SpDefMult,SpDefTime*TimerSum[3],3);
        }
        if(Speed){
            Target.AddStatMultiplier(SpeedMult,SpeedTime*TimerSum[4],4);
        }
        if(HP){
            Target.Character.ChangeHP((int)HPIncrease);
        }
        if(WPUp){
            Target.Character.ChangeWP((int)WPIncrease);
        }
    }
}

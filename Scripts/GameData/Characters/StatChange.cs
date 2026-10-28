using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class StatChange : Resource
{
    [Export] public int AtkTime, DefTime,SpAtkTime,SpDefTime, SpeedTime, StatusTime, DelayTarget;
    [Export] public float AtkMult,DefMult,SpAtkMult,SpDefMult,SpeedMult, HPIncrease, WPIncrease;
    [Export] public bool Atk,Def,Spatk,Spdef,Speed, HP, WPUp, DelayUp, StatCondition, AltStat;
    [Export] public StatusCondition Status;
    [Export] public Array<StatusCondition> Alt;

     public void RaiseStat(BattleCharacter Target, int[] TimerSum = null, BattleCharacter User = null)
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
            Debug.WriteLine("Healing user mult: "+User.StatMultiplier[2]);
            Target.Character.ChangeHP(Mathf.RoundToInt(User.Character.TotalStats.SpAtk * HPIncrease* User.StatMultiplier[2]));
        }
        if(WPUp){
            Target.Character.ChangeWP((int)WPIncrease);
        }
        if (DelayUp)
        {
            Target.CurrentDelay += DelayTarget;
        }
        if (StatCondition)
        {
            Target.Character.status = Status;
        }
        if (AltStat)
        {
            for(int i = 0; i < Alt.Count; i++)
            {
                Target.Character.AltStatus.Add(Alt[i]);                
            }
        }

    }
}

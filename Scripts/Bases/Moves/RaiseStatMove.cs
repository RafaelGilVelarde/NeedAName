using Godot;
using Godot.Collections;
using System;
using System.IO;

[GlobalClass]
public partial class RaiseStatMove : MoveBase
{
    [Export] int AtkTime, DefTime,SpAtkTime,SpDefTime, SpeedTime;
    [Export] float AtkMult,DefMult,SpAtkMult,SpDefMult,SpeedMult, HPIncrease, WPIncrease;
    [Export] bool Atk,Def,Spatk,Spdef,Speed, HP, WPUp;
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
		SceneTreeTimer timer=Users[0].GetTree().CreateTimer(MoveTime,true,true);
        timer.Timeout+=End;

        Users[0].changeAction(BattleCharacter.ActionState.isStatus);
        RaiseStat(Targets[0]);

        void End(){
			BattleManager.instance.CallDeferred("EndMove");
		}
    }
    protected void RaiseStat(BattleCharacter Target){
        if(Atk){
            Target.AddStatMultiplier(AtkMult,AtkTime,0);
        }
        if(Def){
            Target.AddStatMultiplier(DefMult,DefTime,1);
        }
        if(Spatk){
            Target.AddStatMultiplier(SpAtkMult,SpAtkTime,2);
        }
        if(Spdef){
            Target.AddStatMultiplier(SpDefMult,SpDefTime,3);
        }
        if(Speed){
            Target.AddStatMultiplier(SpeedMult,SpeedTime,4);
        }
        if(HP){
            Target.Character.ChangeHP((int)HPIncrease);
        }
        if(WPUp){
            Target.Character.ChangeWP((int)WPIncrease);
        }
    }
}

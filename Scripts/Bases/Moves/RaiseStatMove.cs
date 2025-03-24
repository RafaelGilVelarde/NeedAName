using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class RaiseStatMove : MoveBase
{
    [Export] int AtkTime, DefTime,SpAtkTime,SpDefTime, SpeedTime;
    [Export] float AtkMult,DefMult,SpAtkMult,SpDefMult,SpeedMult;
    [Export] bool Atk,Def,Spatk,Spdef,Speed;
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
		SceneTreeTimer timer=Users[0].GetTree().CreateTimer(MoveTime,true,true);
        timer.Timeout+=End;

        Targets[0].changeAction(BattleCharacter.ActionState.isStatus);
        RaiseStat(Targets[0]);

        void End(){
			BattleManager.instance.CallDeferred("EndMove");
		}
    }
    void RaiseStat(BattleCharacter Target){
        if(Atk){
            Target.AddAtkMultiplier(AtkMult,AtkTime);
        }
        if(Def){
            Target.AddDefMultiplier(DefMult,DefTime);
        }
        if(Spatk){
            Target.AddSpAtkMultiplier(SpAtkMult,SpAtkTime);
        }
        if(Spdef){
            Target.AddSpDefMultiplier(SpDefMult,SpDefTime);
        }
        if(Speed){
            Target.AddSpeedMultiplier(SpeedMult,SpeedTime);
        }
    }
}

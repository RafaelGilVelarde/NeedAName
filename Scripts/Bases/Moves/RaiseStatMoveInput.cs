using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class RaiseStatMoveInput : RaiseStatMove
{
    [Export]int[] TimerSums;
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {

        int[] Sum = { 1, 1, 1, 1, 1 };
        float HPMult = 1.5f;
        bool Succeeded = false;
        Users[0]._Shoot += Countdown;
        SceneTreeTimer timer = Users[0].GetTree().CreateTimer(MoveTime, true, true);
        timer.Timeout += End;

        Users[0].changeAction(BattleCharacter.ActionState.isStatus);
        
        void Countdown(BattleCharacter character)
        {
            character._DoAction += Action;
            character.changeState(BattleCharacter.BattleState.Attacking);
            character._Shoot -= Countdown;
            SceneTreeTimer Timer = character.GetTree().CreateTimer(0.5, true, true);
            character.Character.ShowTextLabel($"{character.Character.Key}", character.Character.Base.TextEffectColor);
            Timer.Timeout += () =>
            {
                if (!Succeeded)
                {
                    character._DoAction -= Action;
                }
            };
        }

        void Action()
        {
            Succeeded = true;
            Users[0]._DoAction -= Action;
            for (int i = 0; i < Sum.Length; i++)
            {
                Sum[i] += TimerSums[i];
            }
            if (Succeeded)
            {
                Users[0].StatMultiplier[2] *= HPMult;
            }
            Stats.RaiseStat(Targets[0], Sum, Users[0]);
        }
        void End()
        {
            if (!Succeeded)
            {
                Stats.RaiseStat(Targets[0],null,Users[0]);                
            }
            else if (Stats.HP)
            {
                Users[0].StatMultiplier[2]/=HPMult;
            }
            BattleManager.instance.CallDeferred("EndMove");
        }

    }

}

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
        bool Succeeded = false;
        Users[0]._Shoot += Countdown;
        SceneTreeTimer timer = Users[0].GetTree().CreateTimer(MoveTime, true, true);
        timer.Timeout += End;

        Users[0].changeAction(BattleCharacter.ActionState.isStatus);

        void Countdown(BattleCharacter character)
        {
            character._DoAction += Action;
            character._Shoot -= Countdown;
            SceneTreeTimer Timer = character.GetTree().CreateTimer(0.2, true, true, true);
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
            RaiseStat(Targets[0], Sum);
        }
        void End()
        {
            if (!Succeeded)
            {
                RaiseStat(Targets[0]);                
            }
            BattleManager.instance.CallDeferred("EndMove");
        }

    }

}

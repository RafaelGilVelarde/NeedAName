using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class Boss1Settings : AISettings
{
    bool Below50;
    [Export] int StatMoveIndex;
    public override Array<float> ChangeMoveChance(BattleCharacter Character)
    {        
        Array<Moves> Moves = Character.Character.Moves;
        Array<float> MoveChances = new Array<float>();
        for (int i = 0; i < Moves.Count; i++)
        { 
            MoveChances.Add(Moves[i].Chance);
        }
        for (int i = 0; i < Moves.Count; i++)
        {
            float Aux = Moves[i].Base.MultiplyChance(Moves[i].Chance, Character);
            CalculateMoveChance(Aux, Moves[i].Chance, i, MoveChances);
        }
        if(Character.Character.stats.HP > Character.Character.stats.MaxHP/2)
        {
            CalculateMoveChance(0,Moves[StatMoveIndex].Chance,StatMoveIndex,MoveChances);
        }
        if(Character.Character.stats.HP <= Character.Character.stats.MaxHP/2 && !Below50)
        {
            CalculateMoveChance(100,Moves[StatMoveIndex].Chance,StatMoveIndex,MoveChances);
            Below50 = true;
        }
        return MoveChances;
    }
    public override void Clear()
    {
        Below50 = false;
    }
}

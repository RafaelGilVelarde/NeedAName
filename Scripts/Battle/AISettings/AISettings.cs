using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class AISettings : Resource
{
    public void CalculateMoveChance(float ChangedChance, float Original, int index, Array<float> MoveChances)
    {
        float Total = 100 - ChangedChance;
        float Difference = ChangedChance - Original;

        MoveChances[index] = ChangedChance;
        if (Total > 0 && Difference != 0)
        {
            float Aux = 100 - Original;
            for (int i = 0; i < MoveChances.Count; i++)
            {
                if (i != index)
                {
                    MoveChances[i] = MoveChances[i] * 100/ Aux;
                    /*if (Difference > 0)
                    {
                        MoveChances[i] += MoveChances[i] / Aux * Difference;
                    }
                    else if (Difference < 0)
                    {
                        MoveChances[i] += (1 - MoveChances[i] / Aux) * Difference;
                    }*/
                }
            }
        }
        else if (Total <= 0)
        {
            for (int i = 0; i < MoveChances.Count; i++)
            {
                if (i != index)
                {
                    MoveChances[i] = 0;
                }
            }
        }
    }
    public virtual Array<float> ChangeMoveChance(BattleCharacter Character)
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
        return MoveChances;
    }
    public virtual Moves MoveChoosing(BattleCharacter Character)
    {
        RandomNumberGenerator RNG = new RandomNumberGenerator();
        int RandomNumber = RNG.RandiRange(1, 100);
        Array<float> Chances = ChangeMoveChance(Character);

        Array<float> ChancesFixed = new Array<float>();
        int MoveIndex = 0;
        float Aux = 0;
        for (int i = 0; i < Chances.Count; i++)
        {
            ChancesFixed.Add(Aux + Chances[i]);
            Aux += Chances[i];
        }

        for (int i = 0; i < ChancesFixed.Count; i++)
        {
            if (RandomNumber <= ChancesFixed[i] && RandomNumber > ChancesFixed[Mathf.Clamp(i-1,0,ChancesFixed.Count)])
            {
                MoveIndex = i;
            }
        }
        //Moves move = Character.Character.Moves[RNG.RandiRange(0, Character.Character.Moves.Count - 1)];
        Moves move = Character.Character.Moves[MoveIndex];
        return move;
    }
    public virtual void UserChoosing(){

    }
    public virtual void TargetChoosing(Moves move, BattleCharacter BattleCharacter){
        RandomNumberGenerator RNG=new RandomNumberGenerator();
        Array<BattleCharacter> TargetCharacters = BattleManager.instance.TargetCharacters;
        if(!move.Base.TargetsParty){
            while(TargetCharacters.Count<move.Base.TargetAmount){
                BattleCharacter target=BattleCharacter.EnemyParty[RNG.RandiRange(0,BattleCharacter.EnemyParty.Count-1)];
                if(!TargetCharacters.Contains(target) && target.Character.status.Base.Name!="KO"){
                    TargetCharacters.Add(target);
                }
            }
           /* for(int i=0;i<move.Base.TargetAmount;i++){
            }*/
        }
        else{
            while(TargetCharacters.Count<move.Base.TargetAmount){
                BattleCharacter target=BattleCharacter.ThisParty[RNG.RandiRange(0,BattleCharacter.ThisParty.Count-1)];
                if(!TargetCharacters.Contains(target)&& target.Character.status.Base.Name!="KO"){
                    TargetCharacters.Add(target);
                }
            }
        }

    }
    public virtual void Clear()
    {
        
    }
}

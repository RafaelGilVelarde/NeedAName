using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
[GlobalClass]
public partial class EnemyCharacterBase : CharacterBase
{
    [Export] public float ExpYield;
    public virtual Moves MoveChoosing(BattleCharacter Character){
        RandomNumberGenerator RNG=new RandomNumberGenerator();
        Moves move=Character.Character.Moves[RNG.RandiRange(0,Character.Character.Moves.Count-1)];
        return move;
    }
    public virtual void UserChoosing(){

    }
    public virtual void TargetChoosing(Moves move, BattleCharacter BattleCharacter){
        RandomNumberGenerator RNG=new RandomNumberGenerator();
        if(!move.Base.TargetsParty){
            for(int i=0;i<move.Base.TargetAmount;i++){
                BattleCharacter target=BattleCharacter.EnemyParty[RNG.RandiRange(0,BattleCharacter.EnemyParty.Count-1)];
                if(!BattleManager.instance.TargetCharacters.Contains(target)){
                    BattleManager.instance.TargetCharacters.Add(target);
                }
            }
        }
        else{
            for(int i=0;i<move.Base.TargetAmount;i++){
                BattleCharacter target=BattleCharacter.ThisParty[(int)RNG.RandiRange(0,BattleCharacter.ThisParty.Count-1)];
                if(!BattleManager.instance.TargetCharacters.Contains(target)){
                    BattleManager.instance.TargetCharacters.Add(target);
                }
            }
        }

    }
    
}

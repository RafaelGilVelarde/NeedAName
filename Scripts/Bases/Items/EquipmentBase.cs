using Godot;
using Godot.Collections;
using System;

public enum EquipmentType{
    Head,
    Body,
    Legs,
    Weapon
}
public enum StatIncrease{
    HP,
    Atk,
    Def,
    SpAtk,
    SpDef,
    Speed
}
[GlobalClass]

public partial class EquipmentBase : ItemBase
{
    [Export] public EquipmentType Type;
    [Export] public Array<StatIncrease> Stat;
    [Export] public PackedScene Animator;
    
    [Export] int StatChange;
    public virtual void FightStartEffect(BattleCharacter Character){

    }
    public virtual void TurnStartEffect(BattleCharacter Character){
        
    }
    public virtual void TurnEndEffect(BattleCharacter Character){
        
    }
    public virtual void ActivateMoveEffect(BattleCharacter Character, MoveBase move){
        
    }
    public virtual void HitEnemyEffect(BattleCharacter Character, MoveBase move){
        
    }
    public virtual void GetHitEffect(BattleCharacter Character,MoveBase Move){
        
    }
}

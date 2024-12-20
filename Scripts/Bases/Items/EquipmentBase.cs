using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

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
    [Export] public EquipmentType EquipType;
    [Export] public Stats stats = new Stats();
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
    public override void Effect(Array<Character> Targets)
    {
        base.Effect(Targets);
        Stats Aux = Targets[0].EquipStats;
        Aux.MaxHP += stats.MaxHP;
        Aux.Atk += stats.Atk;
        Aux.Def += stats.Def;
        Aux.SpAtk += stats.SpAtk;
        Aux.SpDef += stats.SpDef;
        Aux.Speed += stats.Speed;
        Debug.WriteLine("Atk: "+Targets[0].EquipStats.Atk);
    }

    public virtual void UnEquipEffect(Character character){
        Stats Aux = character.EquipStats;
        Aux.MaxHP -= stats.MaxHP;
        Aux.Atk -= stats.Atk;
        Aux.Def -= stats.Def;
        Aux.SpAtk -= stats.SpAtk;
        Aux.SpDef -= stats.SpDef;
        Aux.Speed -= stats.Speed;
    }
}

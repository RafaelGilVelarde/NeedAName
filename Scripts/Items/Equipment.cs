using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

[GlobalClass]
public partial class Equipment : Items
{
    //[Export] public EquipmentBase Base{get; private set;}

    public Equipment (EquipmentBase equipBase, int amount){
        Base = equipBase;
        Amount = amount;
    }
    public Equipment (){

    }


    public override void Use(Array<Character> Targets)
    {
        Targets[0].UnEquip(((EquipmentBase)Base).EquipType);
        Targets[0].Equip(this);
        base.Use(Targets);
    }
    /*protected virtual void Equip(Character character){
        character.Equipment[(int)Base.EquipType] = Base;
        character.SetTotalStats();
        Base.Effect(new Array<Character>{character});
    }*/
}

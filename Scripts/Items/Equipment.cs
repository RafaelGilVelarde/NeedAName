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



    /*protected virtual void Equip(Character character){
        character.Equipment[(int)Base.EquipType] = Base;
        character.SetTotalStats();
        Base.Effect(new Array<Character>{character});
    }*/
}

using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Items: Resource
{
    [Export]public ItemBase Base;
    [Export] public int Amount;

    public virtual void Use(Array<Items> ItemArray, Array<Character> Targets){
        Base.Effect(Targets);
        Amount--;
        if(Amount<=0){
            Amount = 0;
            if(ItemArray.Contains(this)){
                ItemArray.Remove(this);
            }
        }
    }
        public virtual void Toss(Array<Items> ItemArray, int amount){
        Amount-=amount;
        if(Amount<=0){
            Amount = 0;
            if(ItemArray.Contains(this)){
                ItemArray.Remove(this);
            }
        }
    }


}

using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Items: Resource
{
    [Export]public ItemBase Base;
    [Export] public int Amount;

    public virtual void Use(Array<Character> Targets){
        Array<Items> Aux = GameManager.Instance.Data.items[(int)Base.type].items;
        Base.Effect(Targets);
        Amount--;
        if(Amount<=0){
            Amount = 0;
            if(Aux.Contains(this)){
                Aux.Remove(this);
            }
        }
    }
        public virtual void Toss(int amount){
                    Array<Items> Aux = GameManager.Instance.Data.items[(int)Base.type].items;
        Amount-=amount;
        if(Amount<=0){
            Amount = 0;
            if(Aux.Contains(this)){
                Aux.Remove(this);
            }
        }
    }


}

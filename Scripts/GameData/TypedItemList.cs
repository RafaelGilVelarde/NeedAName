using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

[GlobalClass]
public partial class TypedItemList : Resource
{
    [Export] public Array<Items> items;
    [Signal] public delegate void _ItemAddedEventHandler();

    public void AddItem(Items item){
        bool ItemExists = false;
        int ItemIndex = 0;
        for(int i = 0;i < items.Count;i++){
            if(item.Base == items[i].Base){
                ItemExists = true;
                ItemIndex = i;
            }
        }
        if(ItemExists){
            items[ItemIndex].Amount+=item.Amount;
        }
        else{
            items.Add(item);
        }
        EmitSignal_ItemAdded();
    }

}

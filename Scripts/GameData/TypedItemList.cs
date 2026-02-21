using Godot;
using Godot.Collections;
using System;
using System.Linq;

[GlobalClass]
public partial class TypedItemList : Resource
{
    [Export] public Array<Items> items;

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
    }

}

using Godot;
using Godot.Collections;
using System;

public partial class ItemCheckInteract : ItemInteract
{
    [Export] bool TakeItems;

    public override void interact(OverworldController Player)
    {
            bool Check = false;
            int aux = 0;
            Array<Items> Items = GameManager.Instance.Data.items;
            for (int i = 0;i<ItemID.Count;i++){
                for(int j=0;j<Items.Count;j++){
                    if(Items[j].Base.ID == ItemID[i]){
                        aux++;
                    }
                }
            }
            if(aux==ItemID.Count){
                Check = true;
            }
            if(Check){
                TimelineIndex = 2;
            }

            Array<bool> ItemGiven = GameManager.Instance.Data.Flags.ItemGiven;
            for (int i = 0;i<ItemGiven.Count;i++){
                if(ItemGiven[GameID]){
                    TimelineIndex = 3;
                }
            }
        base.interact(Player);
    }
    public override void Action(string argument)
    {
        Array<Items> Items = GameManager.Instance.Data.items;
        if(TakeItems){
            for (int i = 0;i<ItemID.Count;i++){
                for(int j=0;j<Items.Count;j++){
                    if(Items[j].Base.ID == ItemID[i]){
                        Items.RemoveAt(i);
                    }
                }
            }
        }
        base.Action(argument);
    }
}

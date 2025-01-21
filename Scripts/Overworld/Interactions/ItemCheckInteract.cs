using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class ItemCheckInteract : ItemInteract
{
    [Export] bool TakeItems;
    [Export] ItemBase.Type ItemType;

    public override void interact(OverworldController Player)
    {
            bool Check = false;
            int aux = 0;
            Array<Items> Items = GameManager.Instance.Data.items[(int)ItemType].items;
            Array<bool> ItemGiven = GameManager.Instance.Data.Flags.ItemGiven;

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
            if(Check || ItemGiven[GameID]){
                TimelineGroupIndex = 1;
            }

        base.interact(Player);
    }
    public override void Action(string argument)
    {
        switch (argument){
            case "GiveItem":
                Array<Items> Items = GameManager.Instance.Data.items[(int)ItemType].items;
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
            break;
            case "RefuseItem":
                SpokenTo = false;
            break;
        }
    }
}

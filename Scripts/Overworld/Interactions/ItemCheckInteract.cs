using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class ItemCheckInteract : ItemInteract
{
    [Export] bool TakeItems;
    [Export] ItemBase.Type ItemType;

    Array<Items> Aux = new Array<Items>();
    Array<int> AuxAmount = new Array<int>();
    [Export]Array<Items> ItemsToTake = new Array<Items>();

    public override void interact(OverworldController Player)
    {
        Aux.Clear();
        AuxAmount.Clear();
            bool Check = false;
            int aux = 0;
            Array<TypedItemList> ItemList = GameManager.Instance.Data.items;
            Array<bool> ItemGiven = GameManager.Instance.Data.Flags.ItemGiven;

            for (int i = 0;i<ItemsToTake.Count;i++){
                for(int j=0;j<ItemList.Count;j++){
                    Array<Items> Items = GameManager.Instance.Data.items[j].items;
                    for(int k = 0;k<Items.Count;i++){
                        if(Items[k].Base.ID == ItemsToTake[i].Base.ID && Items[k].Amount>=ItemsToTake[i].Amount){
                            Aux.Add(Items[k]);
                            AuxAmount.Add(ItemsToTake[i].Amount);
                            aux++;
                        }
                    }
                }
            }
            if(aux==ItemsToTake.Count){
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
                if(TakeItems){
                    for (int i = 0;i<Aux.Count;i++){
                            Aux[i].Toss(AuxAmount[i]);
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

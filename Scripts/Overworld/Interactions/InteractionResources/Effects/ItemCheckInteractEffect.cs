using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class ItemCheckInteractEffect : ItemInteractEffect
{
     [Export] bool TakeItems;
    Array<Items> Aux = new Array<Items>();
    Array<int> AuxAmount = new Array<int>();
    [Export]Array<Items> ItemsToTake = new Array<Items>();

    public override void ConnectCall()
    {
        Aux.Clear();
        AuxAmount.Clear();
            bool Check = false;
            int aux = 0;
            Array<TypedItemList> ItemList = GameManager.Instance.Data.items;
            //Array<bool> ItemGiven = GameManager.Instance.Data.Flags.ItemGiven;

            for (int i = 0;i<ItemsToTake.Count;i++){
                for(int j=0;j<ItemList.Count;j++){
                    Array<Items> Items = GameManager.Instance.Data.items[j].items;
                    for(int k = 0;k<Items.Count;k++){
                        Debug.WriteLine("InventoryItem: "+Items[k].Base.ID+ " amount: "+Items[k].Amount);
                        Debug.WriteLine("Itemcheck: "+ItemsToTake[i].Base.ID + "amount: "+ItemsToTake[i].Amount);
                        if(Items[k].Base.ID == ItemsToTake[i].Base.ID && Items[k].Amount>=ItemsToTake[i].Amount){
                            Debug.WriteLine("correct");
                            Aux.Add(Items[k]);
                            AuxAmount.Add(ItemsToTake[i].Amount);
                            aux++;
                        }
                    }
                }
            }
            Debug.WriteLine("ItemsCorrect: "+aux);
            Debug.WriteLine("Itemstotal: "+ItemsToTake.Count);
            if(aux==ItemsToTake.Count){
                Check = true;
            }
            DialogicCSharp.instance.SetVariable("ItemCheck","Items",Check);                
            /*for(int i = 0;i<CheckValues.Count;i++){
                TimelineGroupIndex = CheckValues[i].GetTimeline(TimelineGroupIndex);
            }
            if(Check){
                TimelineGroupIndex = 1;
            }*/
        base.ConnectCall();
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
                interact.SpokenTo = false;
            break;
        }
    }
    public override void DisconnectCall()
    {
        DialogicCSharp.instance.SetVariable("ItemCheck","Items",false);                
        base.DisconnectCall();
    }
}

using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class ItemInteract : InteractText
{
    [Export] protected int GameID;
    [Export] protected Array<int> ItemID;
    [Export] bool Delete;
    [Export] protected Array<Items> ItemsToGive;
    Callable DoAction;
    public override void _Ready()
    {
        base._Ready();
        Array<bool> ItemGiven = GameManager.Instance.Data.Flags.ItemGiven;
        if(ItemGiven[GameID]){
            SpokenTo=true;
            Destroy();
        }
        DoAction = new Callable(this,MethodName.Action);
        AnimatorTree?.Set("parameters/conditions/Taken",SpokenTo);
    }
    public override void interact(OverworldController Player)
    {
    
        Node DialogicRoot=DialogicCSharp.instance.DialogicRoot;
        DialogicRoot.Connect("signal_event",DoAction);
        if (ItemsToGive.Count == 1)
        {
            DialogicCSharp.instance.SetVariable("ItemName", "Items", Tr(ItemsToGive[0].Base.Name));
            int aux = ItemsToGive[0].Amount;
            DialogicCSharp.instance.SetVariable("ItemAmount","Number",aux);                
        }
        base.interact(Player);
    }

    public virtual void Action(string argument){
        //Array<Items> Items = GameManager.Instance.Data.items[(int)ItemType].items;
        Array<TypedItemList> ItemLists = GameManager.Instance.Data.items;
        /*if(TakeItems){
            for (int i = 0;i<ItemID.Count;i++){
                for(int j=0;j<Items.Count;j++){
                    if(Items[j].Base.ID == ItemID[i]){
                        Items.RemoveAt(i);
                    }
                }
            }
        }*/
        for(int i = 0;i<ItemsToGive.Count;i++){
            ItemLists[(int)ItemsToGive[i].Base.type].AddItem(ItemsToGive[i]);
            /*Array<Items> Items = ItemLists[(int)ItemsToGive[i].Base.type].items;
            Items.Add(ItemsToGive[i]);*/
        }
        AnimatorTree?.Set("parameters/conditions/Taking",true);
        GameManager.Instance.Data.Flags.ItemGiven[GameID] = true;
    }

    public override void Disable()
    {
        Node DialogicRoot=DialogicCSharp.instance.DialogicRoot;
        DialogicRoot.Disconnect("signal_event",DoAction);
        base.Disable();
        Destroy();
    }
    void Destroy(){
        if(Delete){
            GetParent().QueueFree();
        }
    }
}

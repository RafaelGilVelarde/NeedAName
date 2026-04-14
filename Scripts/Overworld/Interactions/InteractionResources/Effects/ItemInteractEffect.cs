using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class ItemInteractEffect : InteractEffect
{
    [Export] protected int GameID;
    [Export] bool Delete;
    [Export] protected Array<Items> ItemsToGive;
    [Export] protected Interact interact;
    public override void _Ready()
    {
        base._Ready();
        Array<bool> ItemGiven = GameManager.Instance.Data.Flags.ItemGiven;
        if(ItemGiven[GameID]){
            interact.SpokenTo=true;
            Destroy();
        }
        interact.AnimatorTree?.Set("parameters/conditions/Taken",interact.SpokenTo);
    }
    public override void ConnectCall()
    {
        DialogicCSharp.instance.SetVariable("ItemGiven", "Items", GameManager.Instance.Data.Flags.ItemGiven[GameID]);
        if (ItemsToGive.Count == 1)
        {
            DialogicCSharp.instance.SetVariable("ItemName", "Items", Tr(ItemsToGive[0].Base.Name));
            int aux = ItemsToGive[0].Amount;
            DialogicCSharp.instance.SetVariable("ItemAmount","Number",aux);                
        }
        StartInteract = new Callable(this,MethodName.Action);
        base.ConnectCall();
    }

    public virtual void Action(string argument){
        Array<TypedItemList> ItemLists = GameManager.Instance.Data.items;

        for(int i = 0;i<ItemsToGive.Count;i++){
            ItemLists[(int)ItemsToGive[i].Base.type].AddItem(ItemsToGive[i]);
            Debug.WriteLine("Amount: "+ItemLists[(int)ItemsToGive[i].Base.type].items[ItemLists[(int)ItemsToGive[i].Base.type].items.Count-1].Amount);
        }
        interact.AnimatorTree?.Set("parameters/conditions/Taking",true);
        GameManager.Instance.Data.Flags.ItemGiven[GameID] = true;
    }

    public override void DisconnectCall()
    {
        Destroy();
    }
    void Destroy(){
        if(Delete){
            GetParent().GetParent().QueueFree();
        }
    }
}

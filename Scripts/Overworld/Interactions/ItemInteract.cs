using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ItemInteract : Interact
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
        AnimatorTree?.Set("parameters/conditions/Taken",SpokenTo);
    }
    public override void interact(OverworldController Player)
    {
        DoAction = new Callable(this,MethodName.Action);
    
        Node DialogicRoot=DialogicCSharp.instance.DialogicRoot;
        DialogicRoot.Connect("signal_event",DoAction);
        base.interact(Player);
    }

    public virtual void Action(string argument){
        Array<Items> Items = GameManager.Instance.Data.items;
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
            Items.Add(ItemsToGive[i]);
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

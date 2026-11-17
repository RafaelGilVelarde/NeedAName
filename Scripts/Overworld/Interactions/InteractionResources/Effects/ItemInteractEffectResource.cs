using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class ItemInteractEffectResource : InteractEffectResource
{
    [Export] protected string KeyGameID;
    [Export] bool Delete;
    [Export] public Array<Items> ItemsToGive;
    protected Interact interact;
    public override void Ready()
    {
        base.Ready();
        Dictionary<string,Variant> ItemGiven = GameManager.Instance.Data.Flags.Dict;
        if((bool)ItemGiven[KeyGameID])
        {
            interact.SpokenTo=true;
            Destroy();
        }
        interact = (Interact)BaseNode;
        interact.AnimatorTree?.Set("parameters/conditions/Taken",interact.SpokenTo);
    }
    public override void ConnectCall()
    {
        DialogicCSharp.instance.SetVariable("ItemGiven", "Items", GameManager.Instance.Data.Flags.Dict[KeyGameID]);

        DialogicCSharp.instance.SetVariable("ItemCount", "Number", Mathf.CeilToInt(ItemsToGive.Count/3.0f));
        SetItemVariable();
        /*if (ItemsToGive.Count == 1)
        {
            DialogicCSharp.instance.SetVariable("ItemName", "Items", Tr(ItemsToGive[0].Base.Name));
            int aux = ItemsToGive[0].Amount;
            DialogicCSharp.instance.SetVariable("ItemAmount","Number",aux);                
        }*/
        StartInteract = new Callable(this,MethodName.Action);
        base.ConnectCall();
    }

    public virtual void Action(string argument){
        if(argument == "Give")
        {
            Array<TypedItemList> ItemLists = GameManager.Instance.Data.items;

            for(int i = 0;i<ItemsToGive.Count;i++){
                ItemLists[(int)ItemsToGive[i].Base.type].AddItem(ItemsToGive[i]);
                Debug.WriteLine("Amount: "+ItemLists[(int)ItemsToGive[i].Base.type].items[ItemLists[(int)ItemsToGive[i].Base.type].items.Count-1].Amount);
            }
            interact.AnimatorTree?.Set("parameters/conditions/Taking",true);
            GameManager.Instance.Data.Flags.Dict[KeyGameID] = true;            
        }
        if(argument == "Loop")
        {
            SetItemVariable();
        }
    }

    void SetItemVariable()
    {
        DialogicCSharp Instance = DialogicCSharp.instance;
        Instance.ClearItemVariables();
        int AuxIndex = Mathf.CeilToInt(ItemsToGive.Count/3.0f)-(int)Instance.GetVariable("ItemCount","Number");
        Debug.WriteLine("Dialogues: "+(int)Instance.GetVariable("ItemCount","Number"));
        Debug.WriteLine("Dialogues: "+Mathf.CeilToInt(ItemsToGive.Count/3.0f));
        int DialogueIndex = 0;
        for(int i = AuxIndex*3; i < Mathf.Clamp(AuxIndex*3+2,0,ItemsToGive.Count); i++)
        {
            DialogicCSharp.instance.SetVariable("ItemName_"+DialogueIndex, "Items", Tr(ItemsToGive[i].Base.Name));
            int aux = ItemsToGive[i].Amount;
            DialogicCSharp.instance.SetVariable("ItemAmount_"+DialogueIndex,"Number",aux);
            DialogueIndex++;
        }
        DialogicCSharp.instance.SetVariable("ItemCount", "Number", Mathf.Clamp((int)Instance.GetVariable("ItemCount","Number")-1,0,Mathf.Inf));

    }
    
    public override void DisconnectCall()
    {
        DialogicCSharp.instance.ClearItemVariables();
        Destroy();
    }
    void Destroy(){
        if(Delete){
            BaseNode.GetParent().QueueFree();
        }
    }
}

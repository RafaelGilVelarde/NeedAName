using Godot;
using Godot.Collections;
using System;

public partial class TriggerGive : Node
{
    [Export] Array<ObjectToGive> Objects;

    DataManager Data;

    public override void _Ready()
    {
        base._Ready();
        Data =   GameManager.Instance.Data;
        Data.Flags._EventFlagsBoolChanged += GiveObjects;
        Data.Flags._PuzzleFlagsBoolChanged += GiveObjects;
    }

    private void GiveObjects(int Change, bool Changed)
    {
        if (Changed)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (Change == Objects[i].index)
                {
                    switch (Objects[i].objectType)
                    {
                        case ObjectType.Move:
                            int aux = Objects[i].PartyIndex;
                            Data.Party[aux].LearnMove(Objects[i].Move);
                            DialogicCSharp.instance.SetVariable("Moves","MoveName",Objects[i].Move.Base.Name);                
                            break;
                        case ObjectType.Item:
                            Array<TypedItemList> ItemLists = Data.items;
                            int auxItemIndex = (int)Objects[i].Item.Base.type;
                            ItemLists[auxItemIndex].AddItem(Objects[i].Item);
                            DialogicCSharp.instance.SetVariable("ItemName", "Items",Objects[i].Item.Base.Name);                
                            DialogicCSharp.instance.SetVariable("Number","ItemAmount",Objects[i].Item.Amount);                
                            break;
                    }
                    DialogicCSharp.instance.StartDialogue(Objects[i].Timeline, true, false);
                }
            }
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Data.Flags._EventFlagsBoolChanged -= GiveObjects;
        Data.Flags._PuzzleFlagsBoolChanged -= GiveObjects;
    }



}

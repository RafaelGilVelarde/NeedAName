using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class ItemsScreen : MenuScreens
{
    enum State{
        ChooseType,
        ChooseItem,
        ChoiceForItem,
        ChooseCharacter,
        TossItem
    }
        [Export] Control ItemTypes, ChoiceForItem;
        [Export] MenuItemList ItemList;
        [Export] MenuCharacterChooseList CharacterSelect;
        [Export] ItemTossAmountList TossItem;
        [Export] RichTextLabel ItemTitle, ItemDesc;
        int ItemPointerIndex;
        public Items CurrentItem;
        public Character CurrentCharacter;
        public int CurrentType;
        State ItemScreenState;
        Array<State> PreviousState = new Array<State>();
    public override void _Ready()
    {
        base._Ready();
        foreach (BaseButton button in ItemTypes.GetChildren()){
            button.FocusEntered+=()=>ChangeListType(button.GetIndex());
            button.Pressed+=()=>ChangeState(1,true);
        }
    }

    public override void Confirm()
    {
        switch (ItemScreenState){
            case State.ChooseType:
            break;
            case State.ChooseItem:
            break;
            case State.ChooseCharacter:
            break;
            case State.ChoiceForItem:
            break;
        }
    }
    public override void Deny()
    {
        switch (ItemScreenState){
            case State.ChooseType:
                MainMenu.Instance.ChangeState(0);
                PreviousState.Clear();
            break;
            case State.ChooseItem:
                ChangeState((int)PreviousState[PreviousState.Count-1],false);
                PreviousState.RemoveAt(PreviousState.Count-1);
            break;
            case State.ChoiceForItem:
                ChangeState((int)PreviousState[PreviousState.Count-1],false);
                PreviousState.RemoveAt(PreviousState.Count-1);
            break;
            case State.ChooseCharacter:
                ChangeState((int)PreviousState[PreviousState.Count-1],false);
                PreviousState.RemoveAt(PreviousState.Count-1);
            break;
            case State.TossItem:
                ChangeState((int)PreviousState[PreviousState.Count-1],false);
                PreviousState.RemoveAt(PreviousState.Count-1);
            break;
        }
    }
    public override void Select()
    {
        ItemList.FillButtons(0,ScrollList.StartEnd.Regular);
        ChangeState(0,true);
    }
    void ChangeState(int state, bool AdvanceStage){
        if(AdvanceStage){
            PreviousState.Add(ItemScreenState);
        }
        ItemScreenState = (State)state;
        switch (ItemScreenState){
            case State.ChooseType:
                SwitchChildren(ItemTypes,true);
                SwitchChildren(ItemList,false);
                SwitchChildren(ChoiceForItem,false);
                SwitchChildren(CharacterSelect,false);
                SwitchChildren(TossItem,false);

                ItemTypes.GetChild<BaseButton>(CurrentType).GrabFocus();
                TossItem.Visible = false;
                ChoiceForItem.Visible = false;
                CharacterSelect.Visible = false;
            break;
            case State.ChooseItem:
                SwitchChildren(ItemTypes,false);
                SwitchChildren(ItemList,true);
                SwitchChildren(ChoiceForItem,false);
                SwitchChildren(CharacterSelect,false);
                SwitchChildren(TossItem,false);

                ItemList.Buttons[ItemPointerIndex].GrabFocus();
                TossItem.Visible = false;               
                ChoiceForItem.Visible = false;
                CharacterSelect.Visible = false;
            break;
            case State.ChoiceForItem:
                SwitchChildren(ItemTypes,false);
                SwitchChildren(ItemList,false);
                SwitchChildren(ChoiceForItem,true);
                SwitchChildren(CharacterSelect,false);
                SwitchChildren(TossItem,false);
                ChoiceForItem.GetChild<BaseButton>(0).GrabFocus();
                if(CurrentItem.Base.type == ItemBase.Type.Key){
                    ChoiceForItem.GetChild<BaseButton>(1).Visible = false;
                }
                else{
                    ChoiceForItem.GetChild<BaseButton>(1).Visible = true;
                }
                TossItem.Visible = false;
                CharacterSelect.Visible = false;
                ChoiceForItem.Visible = true;
            break;
            case State.ChooseCharacter:
                SwitchChildren(ItemTypes,false);
                SwitchChildren(ItemList,false);
                SwitchChildren(ChoiceForItem,false);
                SwitchChildren(CharacterSelect,true);
                SwitchChildren(TossItem,false);
                CharacterSelect.FillButtons(CharacterSelect.pointerStart,ScrollList.StartEnd.Regular);
                TossItem.Visible = false;
                ChoiceForItem.Visible = false;
                CharacterSelect.Visible = true;
            break;
            case State.TossItem:
                SwitchChildren(ItemTypes,false);
                SwitchChildren(ItemList,false);
                SwitchChildren(ChoiceForItem,false);
                SwitchChildren(CharacterSelect,false);
                SwitchChildren(TossItem,true);
                TossItem.FillButtons(0,ScrollList.StartEnd.Regular);
                ChoiceForItem.Visible = false;
                CharacterSelect.Visible = true;
                TossItem.Visible = true;
            break;
        }
    }
    void SwitchChildren(Control control, bool Switch){
        if(control!=ItemList){
            foreach (Control button in control?.GetChildren()){
                if(Switch){
                    button.FocusMode = FocusModeEnum.All;
                }
                else{
                    button.FocusMode = FocusModeEnum.None;
                }
            }
        }
        else{
            foreach(Control button in ItemList?.Buttons){
                if(Switch){
                    button.FocusMode = FocusModeEnum.All;
                }
                else{
                    button.FocusMode = FocusModeEnum.None;
                }                
            }
        }
    }
    void ChangeListType(int Index){
        ItemList.ItemType = (ItemBase.Type)Index;
        CurrentType = Index;
        ItemList.FillButtons(ItemList.pointerStart,ScrollList.StartEnd.Regular);
    }

    void SetupItemButtons(){
        for(int i = 0;i <ItemList.Buttons.Count;i++){
            MenuItemButtons Aux = ItemList.Buttons[i];
            Aux.Pressed+=()=>SelectItem(Aux);
            Aux.Pressed+=()=>ChangeState(2,true);
        }
    }
    void SetupUseButtons(){
        Array<Node> Buttons = ChoiceForItem.GetChildren();
        ((BaseButton) Buttons[0]).Pressed+=()=>ChangeState(3,true);
        ((BaseButton) Buttons[1]).Pressed+=()=>ChangeState(4,true);
    }
    void SetupCharacterButtons(){
        foreach (MenuCharacterButtons MenuButton in CharacterSelect.Buttons){
            MenuButton.Pressed+=()=>SelectCharacter(MenuButton.character);
            MenuButton.Pressed+=UseItem;
        }
    }
    void SetupTossButtons(){
        TossItem.Amount.Pressed+=Toss;
    }
    void SetupItemDesc(){
        foreach(MenuItemButtons button in ItemList.Buttons){
            button.FocusEntered+=()=>UpdateDescription(button);
        }
    }
    public override void Setup()
    {
        SetupItemButtons();
        SetupUseButtons();
        SetupCharacterButtons();
        SetupTossButtons();
        SetupItemDesc();
    }
    void SelectItem(MenuItemButtons Button){
        ItemPointerIndex = ItemList.Buttons.IndexOf(Button);
        CurrentItem = Button.item;
    }
    void SelectCharacter(Character character){
        CurrentCharacter= character;
    }
    void UpdateDescription(MenuItemButtons Button){
        if(Button?.item?.Base!=null){
            ItemBase item = Button.item.Base;
            ItemTitle.Text = item.Name;
            ItemDesc.Text = item.Description;
        }
    }
    void UseItem(){
        Array<Items> Aux = GameManager.Instance.Data.items[CurrentType].items;
        CurrentItem.Use(new Array<Character>{CurrentCharacter});
        ItemList.FillButtons(ItemList.pointerStart,ScrollList.StartEnd.Regular);   
        if(CurrentItem.Amount == 0){
            CurrentItem = null;
            if(Aux.Count == 0){
                ChangeState(0,false);
            }
            else{
                PreviousState.Clear();
                PreviousState.Add(State.ChooseType);
                ChangeState(1,false);
            }
        }
    }
    void Toss(){
        Array<Items> Aux = GameManager.Instance.Data.items[CurrentType].items;
        CurrentItem.Toss(TossItem.pointerStart);
        if(ItemPointerIndex>0){
            ItemPointerIndex -=1;
        }
        ItemList.FillButtons(0,ScrollList.StartEnd.Regular);   
        TossItem.FillButtons(CurrentItem.Amount, ScrollList.StartEnd.Regular);
        if(CurrentItem.Amount == 0){
            CurrentItem = null;
            if(Aux.Count == 0){
                ChangeState(0,false);
            }
            else{
                PreviousState.Clear();
                PreviousState.Add(State.ChooseType);
                ChangeState(1,false);
            }
        }
    }
}

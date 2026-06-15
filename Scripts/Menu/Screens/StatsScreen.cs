using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class StatsScreen : MenuScreens
{   
    enum State{
        CharacterSelect,
        PartSelect,
        EquipSelect,
        KeySelect,
    }    
    [Export] State StatsScreenState;
    [Export] Control Equipments; 
    [Export] MenuEquipmentList EquipList;
    [Export] BaseButton KeyButton;
    [Export] MenuCharacterList CharacterList;
    [Export] Array<RichTextLabel> StatList;
    [Export] RichTextLabel CharacterName;
    [Export] Array<MenuItemButtons> EquipmentButtons;
    [Export] Array<State> PreviousStates;
    public Character CurrentCharacter;
    public Items CurrentEquipment;
    int CurrentEquipTypeIndex, CurrentCharacterIndex;
    public override void _Ready()
    {
        base._Ready();
    }
    void ChooseCharacter(Character chara){
        CurrentCharacter = chara;
        CurrentCharacterIndex = GameManager.Instance.Data.Party.IndexOf((PartyCharacters)chara);
        DisplayStats();
    }
    void ChangeState(State state, bool AdvanceStage){
        if(AdvanceStage){
            PreviousStates.Add(StatsScreenState);
        }
        StatsScreenState = state;
        switch(StatsScreenState){
            case State.CharacterSelect:
            SwitchChildren(CharacterList,true);
            SwitchChildren(Equipments, false);
            SwitchChildren(EquipList, false);
        
            CharacterList.Buttons[0].GrabFocus();
            KeyButton.GetChild<Node>(0).ProcessMode = ProcessModeEnum.Disabled;
            EquipList.Visible = false;
            break;

            case State.PartSelect:
            SwitchChildren(CharacterList,false);
            SwitchChildren(Equipments, true);
            SwitchChildren(EquipList, false);
            KeyButton.GrabFocus();

            KeyButton.GetChild<Node>(0).ProcessMode = ProcessModeEnum.Disabled;
            EquipList.Visible = false;
            break;

            case State.EquipSelect:
            SwitchChildren(CharacterList,false);
            SwitchChildren(Equipments, false);
            SwitchChildren(EquipList, true);
            EquipList.FillButtons(0,ScrollList.StartEnd.Regular);

            KeyButton.GetChild<Node>(0).ProcessMode = ProcessModeEnum.Disabled;
            EquipList.GetChild<BaseButton>(1).GrabFocus();
            EquipList.Visible = true;
            break;      

            case State.KeySelect:
            SwitchChildren(CharacterList,false);
            SwitchChildren(Equipments, false);
            SwitchChildren(EquipList, false);

            KeyButton.GetChild<Node>(0).ProcessMode = ProcessModeEnum.Inherit;
            EquipList.Visible = false;
            break;         
        }
    }
        void SwitchChildren(Control control, bool Switch){
        //if(control!=ItemList){
            foreach (Control button in control?.GetChildren()){
                if(Switch){
                    button.FocusMode = FocusModeEnum.All;
                }
                else{
                    button.FocusMode = FocusModeEnum.None;
                }
            }
        /*}
        else{
            foreach(Control button in ItemList?.Buttons){
                if(Switch){
                    button.FocusMode = FocusModeEnum.All;
                }
                else{
                    button.FocusMode = FocusModeEnum.None;
                }                
            }
        }*/
    }
    void DisplayStats(){
        Stats stats = CurrentCharacter.stats;
        Stats EquipStats = CurrentCharacter.EquipStats;
        CharacterName.Text = Tr(CurrentCharacter.Base.Name);
        StatList[0].Text = $"{stats.Lv}";
        StatList[1].Text = $"{stats.HP}/{stats.MaxHP} ({EquipStats.MaxHP})";
        StatList[2].Text = $"{stats.Atk} ({EquipStats.Atk})";
        StatList[3].Text = $"{stats.Def} ({EquipStats.Def})";
        StatList[4].Text = $"{stats.SpAtk} ({EquipStats.SpAtk})";
        StatList[5].Text = $"{stats.SpDef} ({EquipStats.SpDef})";
        StatList[6].Text = $"{stats.Speed} ({EquipStats.Speed})";
        ((Button)KeyButton).Text = CurrentCharacter.Key.ToString();


        for(int i = 0;i<EquipmentButtons.Count;i++){
            if(CurrentCharacter.Equipment[i]!=null){
                EquipmentButtons[i].Text = CurrentCharacter.Equipment[i].Name;
            }
            else{
                EquipmentButtons[i].Text = "--";
            }

        }

    }
    public override void Confirm()
    {
        switch (StatsScreenState){
            case State.CharacterSelect:
            break;
            case State.PartSelect:
            break;
            case State.EquipSelect:
            break;
        }
    }
    public override void Deny()
    {
        switch (StatsScreenState){
            case State.CharacterSelect:
                MainMenu.Instance.ChangeState(0);
                PreviousStates.Clear();
            break;
            case State.PartSelect:
                ChangeState(State.CharacterSelect,false);
                PreviousStates.RemoveAt(PreviousStates.Count-1);
            break;
            case State.EquipSelect:
                ChangeState(State.PartSelect,false);
                PreviousStates.RemoveAt(PreviousStates.Count-1);
            break;
        }
    }
    public override void Select()
    {
        base.Select();
        CharacterList.FillButtons(CharacterList.pointerStart,ScrollList.StartEnd.Regular);
        CharacterList.Buttons[0].GrabFocus();
        ((Button)KeyButton).Text = CurrentCharacter.Key.ToString();
    }
    public override void Setup()
    {
        base.Setup();
        SetupPartButton();
        SetupEquipButton();
        SetupCharacterbutton();
        SetupKeyButton();
        
    }
    public override void InitialDisplay()
    {
        base.InitialDisplay();
        bool active = false;
        int aux = 0;
        Character chara = new Character();
        Array<PartyCharacters> characters = GameManager.Instance.Data.Party;
        while(!active){
            chara = characters[(CurrentCharacterIndex+aux)%characters.Count];
            if(!chara.Active){
                aux++;
            }
            else{
                active = true;
            }
        }
        ChooseCharacter(chara);
    }

    void SetupCharacterbutton(){
        CharacterList._ChangeCharacter+=ChooseCharacter;
        CharacterList.Buttons[0].Pressed+=PickCharacter;
    }
    void SetupPartButton(){
        foreach(MenuItemButtons Button in EquipmentButtons){
            Button.Pressed+=()=>ChooseEquipment(Button);
        }
    }
    void SetupEquipButton()
    {
        BaseButton Un = EquipList.GetChild<BaseButton>(1);
        Un.Pressed += Unequip;
        foreach (MenuItemButtons Button in EquipList.Buttons)
        {
            Button.Pressed += () => Equip(Button);
        }
    }
    void SetupKeyButton(){
        KeyButton.Pressed+=()=>ChangeState(State.KeySelect,true);
    }
    void PickCharacter(){
        ChangeState(State.PartSelect,true);
    }
    void ChooseEquipment(MenuItemButtons Button){
        EquipList.EquipType = (EquipmentType)EquipmentButtons.IndexOf(Button);
        CurrentEquipTypeIndex = EquipmentButtons.IndexOf(Button);
        ChangeState(State.EquipSelect,true);
    }
    void Equip(MenuItemButtons Button){
        CurrentEquipment = Button.item;
        //CurrentEquipment.Use(new Array<Character>{CurrentCharacter});
        CurrentCharacter.Equip((Equipment)CurrentEquipment);
        DisplayStats();
        PreviousStates.Clear();
        PreviousStates.Add(State.CharacterSelect);
        ChangeState(State.PartSelect,false);
    }
    void Unequip()
    {
        if (CurrentCharacter.Equipment[CurrentEquipTypeIndex] != null)
        {
            CurrentCharacter.UnEquip((EquipmentType)CurrentEquipTypeIndex);
        }
        DisplayStats();
        PreviousStates.Clear();
        PreviousStates.Add(State.CharacterSelect);
        ChangeState(State.PartSelect,false);      
    }
    public void ChangeKey(InputEvent @event)
    {
        CurrentCharacter.ChangeKey((InputEventKey)@event);
        ((Button)KeyButton).Text = CurrentCharacter.Key.ToString();

        SceneTreeTimer timer = GetTree().CreateTimer(0.2f, true, true);
        timer.Timeout += end;

        void end()
        {
            ChangeState(State.PartSelect, false);
            PreviousStates.RemoveAt(PreviousStates.Count - 1);
        }
    }
}


using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class SaveScreen : MenuScreens
{
    enum State{
        ChooseSave,
        ChooseAction,
        Confirm
    }
    enum Action{
        Save,
        Load,
        Delete
    }
    [Export] MenuSavesList SavesList;
    [Export] State state;
    [Export] Action action;
    [Export] Array<State> PreviousState = new Array<State>();
    [Export] Control ChooseAction, ConfirmAction;
    [Export] DataManager CurrentSave;
    [Export] int SavePointerIndex;

    void ChangeState(State NewState, bool AdvanceStage){
        if(AdvanceStage){
            PreviousState.Add(state);
        }
        state = NewState;
        switch (state){
            case State.ChooseSave:
                SwitchChildren(SavesList,true);
                SwitchChildren(ChooseAction,false);
                SwitchChildren(ConfirmAction,false);
                SavesList.Buttons[SavePointerIndex].GrabFocus();

                SavesList.Visible = true;
                ChooseAction.Visible = false;
                ConfirmAction.Visible = false;
            break;
            case State.ChooseAction:
                SwitchChildren(SavesList,false);
                SwitchChildren(ChooseAction,true);
                SwitchChildren(ConfirmAction,false);
                ChooseAction.GetChild<BaseButton>(0).GrabFocus();

                ChooseAction.Visible = true;
                ConfirmAction.Visible = false;
            break;
            case State.Confirm:
                SwitchChildren(SavesList,false);
                SwitchChildren(ChooseAction,false);
                SwitchChildren(ConfirmAction,true);
                ConfirmAction.GetChild<BaseButton>(0).GrabFocus();

                ChooseAction.Visible = false;
                ConfirmAction.Visible = true;            
                break;
        }
    }

    void SwitchChildren(Control control, bool Switch){
        if(control!=SavesList){
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
            foreach(Control button in SavesList?.Buttons){
                if(Switch){
                    button.FocusMode = FocusModeEnum.All;
                }
                else{
                    button.FocusMode = FocusModeEnum.None;
                }                
            }
        }
    }
    public override void Select()
    {
        base.Select();
        ChangeState(State.ChooseSave,true);
        SavesList.FillButtons(0,ScrollList.StartEnd.Regular);
    }
    public override void Deny()
    {
        base.Deny();
        switch (state){
            case State.ChooseSave:
                MainMenu.Instance.ChangeState(0);
                PreviousState.Clear();
            break;
            case State.ChooseAction:
                ChangeState(State.ChooseSave,false);
            break;
            case State.Confirm:
                ChangeState(State.ChooseAction,false);
            break;
        }
    }
    public override void Setup()
    {
        base.Setup();
        SetupSaveButton();
        SetupChoice();
        SetupConfirm();
    }
    void SetupSaveButton(){
        foreach(MenuSaveButtons button in SavesList.Buttons){
            button.Pressed+=()=>ChooseSave(button,button.GetIndex());
        }
        /*BaseButton SaveButton = ChooseAction.GetChild<BaseButton>(0);
        BaseButton LoadButton = ChooseAction.GetChild<BaseButton>(1);
        BaseButton DeleteButton = ChooseAction.GetChild<BaseButton>(2);*/
    }
    void SetupChoice(){
        foreach(BaseButton button in ChooseAction.GetChildren()){
            button.Pressed+=()=>Choose(button.GetIndex());
        }
        /*BaseButton SaveButton = ChooseAction.GetChild<BaseButton>(0);
        BaseButton LoadButton = ChooseAction.GetChild<BaseButton>(1);
        BaseButton DeleteButton = ChooseAction.GetChild<BaseButton>(2);*/
    }
    void SetupConfirm(){
        BaseButton ConfirmButton = ConfirmAction.GetChild<BaseButton>(0);
        BaseButton DenyButton = ConfirmAction.GetChild<BaseButton>(1);
        DenyButton.Pressed+=Deny;
        ConfirmButton.Pressed+=DoAction;
    }

    void Choose(int index){
        switch (index){
            case 0:
                action = Action.Save;
            break;
            case 1:
                action = Action.Load;
            break;
            case 2:
                action = Action.Delete;
            break;
        }
        ChangeState(State.Confirm,true);
    }    
    void DoAction(){
        Array<DataManager> Saves = GameManager.Instance.Saves;
        int index = Saves.IndexOf(CurrentSave);
        switch (action){
            case Action.Save:
                GameManager.Instance.Save(index);
                ChangeState(State.ChooseSave,false);
            break;
            case Action.Load:
                ChangeState(State.ChooseSave,false);
                MainMenu.Instance.ChangeState(0);
                MainMenu.Instance.OpenCloseMenu(false);
                GameManager.Instance.Load(index);
            break;
            case Action.Delete:
                GameManager.Instance.DeleteSave(index);
            break;
        }
        SavesList.FillButtons(SavesList.pointerStart,ScrollList.StartEnd.Regular);
        PreviousState.Clear();
    }
    void ChooseSave(MenuSaveButtons Button, int SaveIndex){
        SavePointerIndex = SaveIndex;
        CurrentSave = Button.Save;
        ChangeState(State.ChooseAction,true);
    }

}

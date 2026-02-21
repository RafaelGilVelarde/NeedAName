using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Reflection;

public partial class MainMenu : CanvasLayer
{
    enum MenuState{
        MainButtons,
        Menu
    }
    public static MainMenu Instance;
    [Export] Control MainButtons;
    [Export] MenuScreens CurrentScreen;
    [Export] Array<MenuScreens> Screens = new Array<MenuScreens>();
    [Export] int PointerIndex;
    [Export] MenuState State;

    public override void _Ready(){
        Instance = this;
        ReadyMainButtons();
        ProcessMode=ProcessModeEnum.Disabled;
    }
    public void OpenCloseMenu(bool Open){
        Visible = Open;
        EnableMainButtons(Open,0);
        if(!Open){
            GameManager.Instance.controller.SetControllable(true);
            ProcessMode=ProcessModeEnum.Disabled;
        }
        else{
            ProcessMode=ProcessModeEnum.Inherit;
            for(int i = 0;i<Screens.Count;i++){
                if(Screens[i]!=null){
                    Screens[i].InitialDisplay();
                }
            }
        }
    }
    public void ChangeScreen(int Index){
        if (Screens[Index] != null)
        {
            ChangeState(1);
            if(CurrentScreen != null){
                CurrentScreen.Visible = false;
                CurrentScreen.ProcessMode = ProcessModeEnum.Disabled;
            }
            PointerIndex = Index;
            CurrentScreen = Screens[Index];
            CurrentScreen.Visible = true;
            CurrentScreen.ProcessMode = ProcessModeEnum.Inherit;
            CurrentScreen.Select();
            EnableMainButtons(false,0);            
        }
    }

    void ReadyMainButtons(){
        for(int i=0;i<MainButtons.GetChildCount();i++){
            int AuxPointer;
            Button button = MainButtons.GetChild<Button>(i);
            AuxPointer = i;
            button.Pressed+=ChangeScreenToPoint;
            void ChangeScreenToPoint(){
                ChangeScreen(AuxPointer);
            }
        }        

    }
    public void EnableMainButtons(bool Enable, int Pointer){
        foreach(Button button in MainButtons.GetChildren()){
            button.Disabled = !Enable;
            if(Enable){
                button.FocusMode = Control.FocusModeEnum.All;
            }
            else{
                button.FocusMode = Control.FocusModeEnum.None;
            }
        }
        if(Enable){
            MainButtons.GetChild<Button>(Pointer).GrabFocus();
        }
    }
    public void ChangeState(int state){
        State = (MenuState)state;
        if(state == 0){
            EnableMainButtons(true,PointerIndex);
        }   
    }
    public override void _Input(InputEvent @event){
        if(Input.IsActionJustPressed("Confirm")){
            switch(State){
                case MenuState.MainButtons:
                break;
                case MenuState.Menu:
                            CurrentScreen.Confirm();
                break;
            }
		}
		if(Input.IsActionJustPressed("Deny")){
            switch(State){
				case MenuState.MainButtons:
                    OpenCloseMenu(false);
                break;
                case MenuState.Menu:
                    CurrentScreen.Deny();
                break;
            }
		}
    }

}

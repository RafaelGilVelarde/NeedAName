using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

public partial class SettingsScreen : MenuScreens
{
    enum State
    {
        ChooseSettings,
        ChooseLanguage
    }
    [Export] Control Main, Languages;
    [Export] Array<LanguageButtons> languageButtons;
    [Export] Array<State> PreviousState = new Array<State>();
    [Export] State SettingsState;

    public override void Deny()
    {
        base.Deny();
        switch (SettingsState)
        {
            case State.ChooseSettings:
                MainMenu.Instance.ChangeState(0);
                PreviousState.Clear();
            break;
            case State.ChooseLanguage:
                ChangeState(PreviousState[PreviousState.Count],false);
            break;
        }
    }
    public override void Setup()
    {
        base.Setup();
        SetupSettingsButtons();
        SetupLangButtons();

    }
    public override void Select()
    {
        base.Select();
        ChangeState(State.ChooseSettings);
    }
    void ChangeState(State state, bool AdvanceState = true)
    {
        if(AdvanceState){
            PreviousState.Add(state);
        }
        SettingsState = state;
        switch (state)
        {
            case State.ChooseSettings:
                SwitchChildren(Main,true);
                SwitchChildren(Languages,false);
                Languages.Hide();
                Main.GetChild<BaseButton>(0).GrabFocus();
            break;
            case State.ChooseLanguage:
                SwitchChildren(Main,false);
                SwitchChildren(Languages,true);
                Languages.Show();
                languageButtons[0].GrabFocus();
            break;
        }
    }

    void SwitchChildren(Control control, bool Switch){
        foreach (Control button in control?.GetChildren()){
                if(Switch){
                    button.FocusMode = FocusModeEnum.All;
                }
                else{
                    button.FocusMode = FocusModeEnum.None;
                }
            }
    }


    void SetupSettingsButtons()
    {
        for(int i = 0; i < Main.GetChildCount(); i++)
        {
            BaseButton button = Main.GetChild<BaseButton>(i);
            button.Pressed+=()=>{
                ChangeState((State)i);
                Debug.WriteLine("changing to: "+(State)i);
            };
        }
        
    }
    void SetupLangButtons()
    {
        foreach (LanguageButtons button in languageButtons)
        {
            button.Setup();
            button.Pressed+=()=>{
                ChangeState(State.ChooseSettings,false);
                PreviousState.Clear();
            };
        }
    }
}

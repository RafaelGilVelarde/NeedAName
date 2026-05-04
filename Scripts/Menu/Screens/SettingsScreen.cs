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
        ChooseLanguage,
        ChooseVolumeMixer,
        ChooseVolume
        
    }
    [Export] Control Main, Languages, Volumes;
    [Export] Array<LanguageButtons> languageButtons;
    [Export] Array<VolumeSlider> VolumeSliders;
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
                ChangeState(PreviousState[PreviousState.Count-1],false);
                PreviousState.RemoveAt(PreviousState.Count-1);
            break;
            case State.ChooseVolumeMixer:
                ChangeState(PreviousState[PreviousState.Count-1],false);
                PreviousState.RemoveAt(PreviousState.Count-1);
            break;
            case State.ChooseVolume:
                ChangeState(PreviousState[PreviousState.Count-1],false);
                PreviousState.RemoveAt(PreviousState.Count-1);
            break;
        }
    }
    public override void Setup()
    {
        base.Setup();
        SetupSettingsButtons();
        SetupLangButtons();
        SetupVolButtons();

    }
    public override void Select()
    {
        base.Select();
        ChangeState(State.ChooseSettings);
    }
    void ChangeState(State state, bool AdvanceState = true)
    {
        if(AdvanceState){
            PreviousState.Add(SettingsState);
        }
        SettingsState = state;
        switch (state)
        {
            case State.ChooseSettings:
                SwitchChildren(Main,true);
                SwitchChildren(Languages,false);
                Languages.Hide();
                Volumes.Hide();
                Main.GetChild<BaseButton>(0).GrabFocus();
            break;
            case State.ChooseLanguage:
                SwitchChildren(Main,false);
                SwitchChildren(Languages,true);
                Languages.Show();
                languageButtons[0].GrabFocus();
            break;
            case State.ChooseVolumeMixer:
                SwitchChildren(Main,false);
                SwitchChildren((Control)Volumes.GetChild(0),true);
                SwitchChildren((Control)Volumes.GetChild(1),true);
                Volumes.Show();
                VolumeSliders[0].SelectButton.GrabFocus();

                for(int i = 0; i < VolumeSliders.Count; i++)
                {
                    VolumeSliders[i].ShowValue();
                    VolumeSliders[i].Disable();
                }
            break;
            case State.ChooseVolume:
                SwitchChildren(Main,false);
                SwitchChildren((Control)Volumes.GetChild(0),false);
                SwitchChildren((Control)Volumes.GetChild(1),false);
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
        foreach (BaseButton button in Main.GetChildren())
        {
            button.Pressed += () =>
            {
                Debug.WriteLine(Main.GetChildren().IndexOf(button)+1);
                ChangeState((State)(Main.GetChildren().IndexOf(button)+1));
            };
        }
        /*for(int i = 0; i < Main.GetChildCount(); i++)
        {
            BaseButton button = Main.GetChild<BaseButton>(i);
            Debug.WriteLine(((Button)button).Text + ": "+(State)i+1);
            button.Pressed+=()=>{
                ChangeState((State)i+1);
                Debug.WriteLine("changing to: "+(State)i+1);
            };
        }*/
        
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
    void SetupVolButtons()
    {
        foreach (VolumeSlider slider in VolumeSliders)
        {
            Debug.WriteLine(slider);
            Debug.WriteLine("slider set up: "+slider.SelectButton);
            slider.SelectButton.Pressed += () =>
            {
                Debug.WriteLine("PressedSelect");
                ChangeState(State.ChooseVolume);
            };
        }
    }
}

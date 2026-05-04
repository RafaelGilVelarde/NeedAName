using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

enum StartMenuState
{
    LanguageSelect,
    AudioMixerSelect,
    VolumeSelect,
    Start,
    SavesList
}
public partial class StartMenu : Node
{
    [Export] BaseButton Start, Continue, Language, Volume;
    [Export] Control Menu, LanguageMenu, VolumeMenu;
    [Export] Array<LanguageButtons> Languages;
    [Export] MenuSavesList SavesList;
    [Export] PackedScene NewGameScene;
    [Export] Color TransitionColor = Colors.Black;
    [Export] Node2D MainSprite;
    [Export] StartMenuState State;
    [Export] RichTextLabel PlayerName; 
    [Export] Array<int> AudioIndex;
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        switch (State)
        {
            case StartMenuState.Start:
                break;
            case StartMenuState.SavesList:
                if (Input.IsActionJustPressed("Deny"))
                {
                    MainMenu();
                }
                break;
            case StartMenuState.LanguageSelect:
                if (Input.IsActionJustPressed("Deny"))
                {
                    MainMenu();
                }
                break;
            case StartMenuState.AudioMixerSelect:
                if (Input.IsActionJustPressed("Deny"))
                {
                    MainMenu();
                }
                break;
            case StartMenuState.VolumeSelect:
                if (Input.IsActionJustPressed("Deny"))
                {
                    ShowAudioMixerList();
                    foreach (VolumeSlider slider in VolumeMenu.GetChildren())
                    {
                        slider.Disable();
                    }  
                }
                break;
        }
    }

    public override void _Ready()
    {
        GameManager Game = new GameManager();
        SceneTreeTimer Timer = GetTree().CreateTimer(0.3, true, true, true);
        Timer.Timeout += () =>
        {
            Game = GameManager.Instance;
            Flags flags = Game.Data.Flags;
            if (flags.EventFlags[0])
            {
                MainSprite.Show();
            }
            PlayerName.Text = $"[center]{Game.Settings.DisplayName}[/center]";
            Game.GetWindow().Title = Game.Settings.DisplayName;
            Game.PlayTransition( Color.Color8(0,0,0,0));
            Game.TransitionTween.Finished+=Setup;
            Game.StopAudio();
        };
    }
    void Setup()
    {
        Start.Pressed += StartNewGame;
        Continue.Pressed += LoadSavesList;
        Language.Pressed += ShowLanguageList;
        Volume.Pressed += ShowAudioMixerList;
        
        GameManager.Instance.TransitionTween.Finished -= Setup;
        SetupSavesButtons();
        SetupLanguageButtons();
        SetupVolumeButtons();
        if (GameManager.Instance.SavesExist)
        {
            MainMenu();
            Start.GrabFocus();
            GameManager.Instance.PlayAudio(AudioIndex[1%AudioIndex.Count]);
        }
        else
        {
            ShowLanguageList();
            GameManager.Instance.PlayAudio(AudioIndex[0%AudioIndex.Count]);
        }
    }

    void StartNewGame()
    {
        Menu.Hide();
        GameManager.Instance.PlayTransition(TransitionColor);
        GameManager.Instance.TransitionTween.Finished += () => GetTree().ChangeSceneToPacked(NewGameScene);
        //SceneTreeTimer timer = GetTree().CreateTimer(0.4f);
        //timer.Timeout += ()=>GetTree().ChangeSceneToPacked(NewGameScene);
    }
    void LoadSavesList()
    {
        State = StartMenuState.SavesList;
        SavesList.Show();
        SavesList.FillButtons(0, ScrollList.StartEnd.Regular);
        SavesList.Buttons[0].GrabFocus();
        LanguageMenu.Hide();
        Menu.Hide();
    }
    void ShowLanguageList()
    {
        State = StartMenuState.LanguageSelect;
        LanguageMenu.Show();
        SavesList.Hide();
        Menu.Hide();
        Languages[0].GrabFocus();

    }
    void ShowAudioMixerList()
    {
        State = StartMenuState.AudioMixerSelect;
        VolumeMenu.Show();
        SavesList.Hide();
        Menu.Hide();
        foreach (VolumeSlider slider in VolumeMenu.GetChildren())
        {
            slider.SelectButton.FocusMode = Control.FocusModeEnum.All;
        }     
        ((VolumeSlider)VolumeMenu.GetChild(0)).SelectButton.GrabFocus();

    }
    void VolumeSelect()
    {
        State = StartMenuState.VolumeSelect;
        foreach (VolumeSlider slider in VolumeMenu.GetChildren())
        {
            slider.SelectButton.FocusMode = Control.FocusModeEnum.None;
        }     

    }
    void SetupSavesButtons()
    {
        Array<MenuSaveButtons> Buttons = SavesList.Buttons;
        foreach (MenuSaveButtons button in SavesList.Buttons)
        {
            button.Pressed += () => LoadGame(button);
        }
    }
    void SetupLanguageButtons()
    {
        foreach (LanguageButtons button in Languages)
        {
            button.Setup();
            button.Pressed+=MainMenu;
        }       
    }
    void SetupVolumeButtons()
    {
        foreach (VolumeSlider slider in VolumeMenu.GetChildren())
        {
            slider.SelectButton.Pressed += VolumeSelect;
        }       
    }
    
    void LoadGame(MenuSaveButtons button)
    {
        Menu.Hide();
        GameManager Game = GameManager.Instance;
        int Index = Game.Saves.IndexOf(button.Save);
        Game.Load(Index);
        SavesList.Hide();
    }
    void MainMenu()
    {
        State = StartMenuState.Start;
        LanguageMenu.Hide();
        VolumeMenu.Hide();
        SavesList.Hide();
        Menu.Show();
        Continue.GrabFocus();
    }

}

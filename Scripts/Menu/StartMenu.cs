using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

enum StartMenuState
{
    LanguageSelect,
    Start,
    SavesList
}
public partial class StartMenu : Node
{
    [Export] BaseButton Start, Continue, Language;
    [Export] Control Menu, LanguageMenu;
    [Export] Array<LanguageButtons> Languages;
    [Export] MenuSavesList SavesList;
    [Export] PackedScene NewGameScene;
    [Export] Color TransitionColor = Colors.Black;
    [Export] Node2D MainSprite;
    [Export] StartMenuState State;
    [Export] RichTextLabel PlayerName; 
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
        GameManager.Instance.TransitionTween.Finished -= Setup;
        SetupSavesButtons();
        SetupLanguageButtons();
        if (GameManager.Instance.SavesExist)
        {
            MainMenu();
            Start.GrabFocus();
        }
        else
        {
            ShowLanguageList();
        }
    }

    void StartNewGame()
    {
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
    
    void LoadGame(MenuSaveButtons button)
    {
        GameManager Game = GameManager.Instance;
        int Index = Game.Saves.IndexOf(button.Save);
        Game.Load(Index);
        SavesList.Hide();
    }
    void MainMenu()
    {
        State = StartMenuState.Start;
        LanguageMenu.Hide();
        SavesList.Hide();
        Menu.Show();
        Continue.GrabFocus();
    }

}

using Godot;
using Godot.Collections;
using System;

enum StartMenuState
{
    Start,
    SavesList
}
public partial class StartMenu : Node
{
    [Export] BaseButton Start, Continue;
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
        }
    }

    public override void _Ready()
    {
        GameManager Game = new GameManager();
        SceneTreeTimer Timer = GetTree().CreateTimer(0.3, true, true, true);
        Timer.Timeout += () =>
        {
            Game = GameManager.Instance;
            Flags flags = Game.Saves[Game.CurrentSave].Flags;
            if (flags.EventFlags[0])
            {
                MainSprite.Show();
            }
            PlayerName.Text = $"[center]{Game.Saves[Game.CurrentSave].Party[0].Name}[/center]";
            Game.PlayTransition( Color.Color8(0,0,0,0));
            Game.TransitionTween.Finished+=Setup;
        };
    }
    void Setup()
    {
        Start.Pressed += StartNewGame;
        Continue.Pressed += LoadSavesList;
        GameManager.Instance.TransitionTween.Finished -= Setup;
        SetupSavesButtons();
        Start.GrabFocus();
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
        Start.Hide();
        Continue.Hide();
    }
    void SetupSavesButtons()
    {
        Array<MenuSaveButtons> Buttons = SavesList.Buttons;
        foreach (MenuSaveButtons button in SavesList.Buttons)
        {
            button.Pressed += () => LoadGame(button);
        }
    }
    void LoadGame(MenuSaveButtons button)
    {
        GameManager Game = GameManager.Instance;
        int Index = Game.Saves.IndexOf(button.Save);
        GameManager.Instance.Load(Index);
    }
    void MainMenu()
    {
        State = StartMenuState.Start;
        SavesList.Hide();
        Start.Show();
        Continue.Show();
    }

}

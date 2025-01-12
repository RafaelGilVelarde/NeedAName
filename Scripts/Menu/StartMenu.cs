using Godot;
using Godot.Collections;
using System;

public partial class StartMenu : Node
{
    [Export] BaseButton Start, Continue;
    [Export] MenuSavesList SavesList;
    [Export] PackedScene NewGameScene;
    [Export] Color TransitionColor = Colors.Black;

    public override void _Ready()
    {
        Setup();
    }
    void Setup(){
        Start.Pressed+=StartNewGame;
        Continue.Pressed+=LoadSavesList;
        SetupSavesButtons();
        Start.GrabFocus();
    }

    void StartNewGame(){
        GameManager.Instance.PlayTransition(TransitionColor);
        GameManager.Instance.TransitionTween.Finished+=()=>GetTree().ChangeSceneToPacked(NewGameScene);
        //SceneTreeTimer timer = GetTree().CreateTimer(0.4f);
        //timer.Timeout += ()=>GetTree().ChangeSceneToPacked(NewGameScene);
    }
    void LoadSavesList(){
        SavesList.Visible = true;
        SavesList.FillButtons(0,ScrollList.StartEnd.Regular);
        SavesList.Buttons[0].GrabFocus();
    }
    void SetupSavesButtons(){
        Array<MenuSaveButtons> Buttons = SavesList.Buttons;
        foreach(MenuSaveButtons button in SavesList.Buttons){
            button.Pressed+=()=>LoadGame(button);
        }
    }
    void LoadGame(MenuSaveButtons button){
        GameManager Game = GameManager.Instance;
        int Index = Game.Saves.IndexOf(button.Save);
        GameManager.Instance.Load(Index);
    }

}

using Godot;
using System;

public partial class LossScreen : Control
{
    [Export] Button Reload, TitleScreen, Retry;
    BattleManager Battle;
    GameManager Game;
    public override void _Ready()
    {
        base._Ready();
        Reload.Pressed+=SetupReload;
        TitleScreen.Pressed+=SetupReturnTitleScreen;
        Retry.Pressed+=SetupRetry;
        Battle=BattleManager.instance;
        Game = GameManager.Instance;
    }

    public void OpenScreen(){
        Visible = true;
        Retry.GrabFocus();
    }
    void SetupReload(){
        Battle.Clear();
        Game.Load(Game.CurrentSave);
        Battle.Scene = null;
        Visible = false;
    }
    void SetupReturnTitleScreen(){
        Battle.Clear();
        Game.Restart();
        Visible = false;
    }
    void SetupRetry(){
        Battle.RetryBattle();
        Visible = false;
    }

}

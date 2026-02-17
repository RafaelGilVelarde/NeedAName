using Godot;
using System;

[GlobalClass]
public partial class Boss1 : BattleScene
{
    [Export] int Scene;
    [Export] Areas Area;
    [Export] PackedScene PackedScene;
    [Export] Color TransitionColor;
    [Export] string VariableName, VariableFolder;
    protected override void EndPostBattle()
    {
        base.EndPostBattle();
        GameManager.Instance.Data.Flags.ChangeBoolFlag(5, true, FlagType.Event);
        //GameManager.Instance.SwitchScene(Scene, (int)Area, Vector2.Zero, TransitionColor);
        DialogicCSharp Dialogic = DialogicCSharp.instance;
        GameManager Game = GameManager.Instance;
        Game.Data.Party[0].Name = Dialogic.GetVariable(VariableName, VariableFolder);
        Dialogic.SetVariable(VariableName, VariableFolder, "");
        Game.GetTree().ChangeSceneToPacked(PackedScene);

        Game.Characters.Clear();
        Game.controller = null;
        Game.OverworldCam = null;
        Game.BattleCam = null;
        Game.Data.Position = Vector2.Zero;
        Game.Data.AreaIndex = (int)Area;
        Game.Data.Scene = Scene;
        Game.Settings.DisplayName = Game.Data.Party[0].Name;
        //Game.Save(Game.Settings.CurrentSave);
        //GameManager.Instance.GetTree().ChangeSceneToPacked(PackedScene);

    }
}

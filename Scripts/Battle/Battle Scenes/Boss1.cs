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
        
        Game.Data.Position = Vector2.Zero;
        Game.Data.AreaIndex = (int)Area;
        Game.Data.Scene = Scene;
        Game.Save(Game.CurrentSave);
        //GameManager.Instance.GetTree().ChangeSceneToPacked(PackedScene);

    }
}

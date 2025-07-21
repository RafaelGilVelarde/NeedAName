using Godot;
using System;

public partial class WaterEnd : Node
{
    [Export] string Timeline,VariableName, VariableFolder;
    [Export] PackedScene PackedScene;
    Callable EndDialogueTimeline;
    public override void _Ready()
    {
        base._Ready();
        DialogicCSharp Dialogic = DialogicCSharp.instance;

        EndDialogueTimeline = new Callable(this, MethodName.EndDialogue);
        Dialogic.DialogicRoot.Connect("timeline_ended", EndDialogueTimeline);
        Dialogic.StartDialogue(Timeline, true, false);
    }

    void EndDialogue()
    {
        DialogicCSharp Dialogic = DialogicCSharp.instance;
        GameManager.Instance.Data.Party[0].Name = Dialogic.GetVariable(VariableName, VariableFolder);
        Dialogic.SetVariable(VariableName, VariableFolder, "");
        GetTree().ChangeSceneToPacked(PackedScene);
    }

}

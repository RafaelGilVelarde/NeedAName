using Godot;
using System;
using System.Diagnostics;

public enum SwitchType{
    Bool,
    Int
}
public partial class Switch : InteractText
{
    [Export] bool Toggleable;
    [Export] PuzzleCheck Check;
    [Export] bool CheckAtStart = true, HasDialogue;
    [Export] int SwitchState, MaxStates;
    public override void _Ready()
    {
        base._Ready();
        if(CheckAtStart){
            SwitchState = (int)GameManager.Instance.Data.Flags.Dict[Check.KeyIndex];
            Press(SwitchState % MaxStates);
        }
    }

    public override void interact(OverworldController Player)
    {
        int StateAux = 0;
        if (Toggleable)
        {
            SwitchState++;
        }
        else
        {
            SwitchState = 1;
        }

        StateAux = SwitchState % MaxStates;
        Press(StateAux);
        if(HasDialogue){
            base.interact(Player);
        }
    }
    void Press(int State){
        switch (visualType){
            case VisualType.Animation:
                AnimatorTree.Set("parameters/State",State);
            break;
            case VisualType.Color:
                MainSprite.Modulate = Colors[State];
            break;
            case VisualType.Sprite:
                MainSprite = Sprites[State];
            break;
        }

        Flags Aux = GameManager.Instance.Data.Flags;
        Aux.ChangeDictFlag(Check.KeyIndex,State);
        if (State > 0)
        {
            SpokenTo = true;
        }
        //Check.ActivateEffect();
    }
}


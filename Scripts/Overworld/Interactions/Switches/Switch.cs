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
    [Export] SwitchType Type;
    [Export] int SwitchState, MaxStates;
    public override void _Ready()
    {
        base._Ready();
        if(CheckAtStart){
            switch (Type){
                case SwitchType.Bool:
                    MaxStates = 2;
                    if(!GameManager.Instance.Data.Flags.PuzzleFlags[Check.FlagIndex]){
                        SwitchState = 0;
                    }
                    else{
                        SwitchState = 1;
                    }
                break;
                case SwitchType.Int:
                    SwitchState = GameManager.Instance.Data.Flags.PuzzleIntFlags[Check.FlagIndex];
                break;
            }
            Press(SwitchState % MaxStates);
        }
    }

    public override void interact(OverworldController Player)
    {
        int StateAux = 0;
        switch (Type){
            case SwitchType.Bool:
            if(Toggleable){
                SwitchState++;
            }
            else{
                SwitchState = 1;
            }
            break;
            case SwitchType.Int:
            SwitchState++;
            break;
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
        int FlagIndex = Check.FlagIndex;
        switch (Type){
            case SwitchType.Bool:
                bool Pressed = State == 1;
                Aux.ChangeBoolFlag(FlagIndex,Pressed,FlagType.Puzzle);
                SpokenTo = Pressed;
               /* else{
                    if(!Aux.PuzzleFlags[FlagIndex]){
                        Aux.ChangeBoolFlag(FlagIndex,true,FlagType.Puzzle);       
                    }
                }*/
            break;
            case SwitchType.Int:
                Aux.ChangeIntFlag(FlagIndex,State,FlagType.Puzzle);
            break;
        }
        //Check.ActivateEffect();
    }
}


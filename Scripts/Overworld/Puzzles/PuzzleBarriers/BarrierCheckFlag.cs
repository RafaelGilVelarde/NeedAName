using Godot;
using System;
using System.Diagnostics;

public partial class BarrierCheckFlag : PuzzleCheck
{
    [Export] AnimationPlayer Animator;
    [Export] CollisionShape2D Shape;
    [Export] string DisableAnimation, Dialogue = "";
    [Export] int CheckIntFlag;
    public override void Check(int Index, bool Changed){
        Debug.WriteLine("Checking: "+Index +" Changed: "+Changed);
        if(FlagIndex == Index){
            if(GameManager.Instance.Data.Flags.PuzzleFlags[FlagIndex]){
                Debug.WriteLine("Activating");
                ActivateEffect();
            }
        }
    }
    public override void CheckInt(int Index, bool Changed)
    {
        if(Index == FlagIndex){
            if(GameManager.Instance.Data.Flags.PuzzleIntFlags[Index] == CheckIntFlag){
                ActivateEffect();
            }
        }    
    }
    public override void ActivateEffect(){
        //Shape.Disabled = true;
        Debug.WriteLine("Puzzle: "+GameManager.Instance.Data.Flags.PuzzleFlags[FlagIndex] + "Index: "+ FlagIndex);
        if(Animator!=null){
            Animator?.Play(DisableAnimation);
            Animator.Autoplay = "";
        }
        /*if(HasDialogue){
            DialogicCSharp Dialog=DialogicCSharp.instance;
            Dialog.StartDialogue(Dialogue,true,false);
        }*/
    }
    
}
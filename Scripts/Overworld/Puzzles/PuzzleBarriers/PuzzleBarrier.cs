using Godot;
using System;

public partial class PuzzleCheck : Node2D
{
    [Export] bool CheckAtStart;

    public override void _Ready()
    {
        if(CheckAtStart){
            if(Check()){
                ActivateEffect();
            }
        }
    }
    public virtual bool Check(){
        return false;
    }

    public virtual void ActivateEffect(){

    }
}

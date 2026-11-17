using Godot;
using System;

public partial class CutsceneInteractEffect : InteractEffect
{
    [Export] protected CutscenePlayer CutsceneAnimator;

    public override void ConnectCall()
    {
        base.ConnectCall();
        StartInteract = new Callable(this,MethodName.CheckCutscene);
    }

    protected virtual void CheckCutscene(string Cutscene)
    {
        if (CutsceneAnimator.CutsceneNames.Contains(Cutscene))
        {
            CutsceneAnimator.PlayAnimation(Cutscene);
        }
        
        if (Cutscene == "EndCutscene")
        {
            CutsceneAnimator.EndAnimation();
        }
    }
}

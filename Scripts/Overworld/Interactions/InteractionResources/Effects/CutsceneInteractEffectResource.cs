using Godot;
using System;

[GlobalClass]
public partial class CutsceneInteractEffectResource : InteractEffectResource
{
    //    [Export] protected CutscenePlayer CutsceneAnimator;

    public override void ConnectCall()
    {
        base.ConnectCall();
        StartInteract = new Callable(this,MethodName.CheckCutscene);
    }

    protected virtual void CheckCutscene(string Cutscene)
    {
        if (Scene.CurrentScene.CutsceneAnimator.CutsceneNames.Contains(Cutscene))
        {
            Scene.CurrentScene.CutsceneAnimator.PlayAnimation(Cutscene);
        }
        
        if (Cutscene == "EndCutscene")
        {
            Scene.CurrentScene.CutsceneAnimator.EndAnimation();
        }
    }
}

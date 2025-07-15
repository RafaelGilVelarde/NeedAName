using Godot;
using Godot.Collections;
using System;

public partial class CutscenePlayer : AnimationPlayer
{
    [Export] public Array<Vector2> InitialPositions;
    [Export] public Array<int> InitialPositionStartIndex;
    [Export] public Array<float> PositioningDurations, CutsceneDurations;
    [Export] public Array<string> CutsceneNames;

    public virtual void PlayAnimation(string Animation)
    {

        int Index = CutsceneNames.IndexOf(Animation);
        GameManager Game = GameManager.Instance;
        Game.Leader.OverworldCollider.Disabled = true;

        Tween tween = CreateTween();
        tween.SetParallel(true);

        for (int i = 0; i < Game.Followers.Count; i++)
        {
            tween.TweenProperty(Game.Followers[i].Parent, "position", InitialPositions[i + InitialPositionStartIndex[Index]], 0.2);
        }
        tween.Finished += () =>
        {
            Play(Animation);
            tween.Kill();
        };
    }
    public virtual void EndAnimation()
    {
        Stop();
    }
}

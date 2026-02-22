using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class CutscenePlayer : AnimationPlayer
{
    [Export] public Array<Vector2> InitialPositions;
    [Export] public Array<int> InitialPositionStartIndex;
    [Export] public Array<float> PositioningDurations, CutsceneDurations;
    [Export] public Array<string> CutsceneNames;

    public virtual void PlayAnimation(string Animation, bool ChangePos = true)
    {
        Debug.WriteLine("Cutscene: "+Animation);

        int Index = CutsceneNames.IndexOf(Animation);
        GameManager Game = GameManager.Instance;
        Game.Leader.OverworldCollider.Disabled = true;
        Game.OverworldCam.PositionSmoothingEnabled = false;
        Game.BattleCam.PositionSmoothingEnabled = false;
        Game.OverworldCam.Reparent(Game.OverworldCam.CameraParent);
        Game.BattleCam.Reparent(Game.OverworldCam.CameraParent);

        Tween tween = CreateTween();
        tween.SetParallel(true);

        for (int i = 0; i < Game.Followers.Count; i++)
        {
            if (ChangePos)
            {
                tween.TweenProperty(Game.Followers[i].Parent, "global_position", InitialPositions[i + InitialPositionStartIndex[Index%InitialPositionStartIndex.Count]], 0.2);                
            }
            else
            {
                tween.TweenProperty(Game.Followers[i].Parent, "global_position", Game.Followers[i].Parent.GlobalPosition, 0.2);
            }
        }
        tween.Finished += () =>
        {
            Game.OverworldCam.PositionSmoothingEnabled = true;
            Game.BattleCam.PositionSmoothingEnabled = true;
            Play(Animation);
            tween.Kill();
        };
    }
    public virtual void EndAnimation()
    {
        Debug.WriteLine("End Animation");
        GameManager Game = GameManager.Instance;
        Game.SetCamera(Game.OverworldCam);
        Game.SetBattleCamera(Game.BattleCam);
        Game.OverworldCam.PositionSmoothingEnabled = false;
        Game.BattleCam.PositionSmoothingEnabled = false;

        Tween CamTween = CreateTween();
        CamTween.SetParallel(true);

        CamTween.TweenProperty(Game.OverworldCam, "position", Vector2.Zero, 0.2);
        CamTween.Finished += () =>
        {
            Game.OverworldCam.PositionSmoothingEnabled = true;
            Game.BattleCam.PositionSmoothingEnabled = true;
            CamTween.Kill();
        };
    }
    public void PlayAudio(int AudioIndex)
    {
        GameManager.Instance.PlayAudio(AudioIndex);
    }
    public void StopAudio()
    {
        GameManager.Instance.StopAudio();
    }
}

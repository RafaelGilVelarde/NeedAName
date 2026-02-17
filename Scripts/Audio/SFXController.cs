using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class SFXController : Node
{
    [Export] public Array<AudioStreamPlayer2D> SFXPlayers;
    [Export] public Array<SFXList> Lists;

    public void PlaySFX(int SFXPlayer, int SFX)
	{
		SFXPlayers[SFXPlayer%SFXPlayers.Count].VolumeDb = 0;
		
		SFXList Aux = Lists[SFXPlayer%Lists.Count];
		SFXPlayers[SFXPlayer%SFXPlayers.Count].Stream = Aux.SoundEffects[SFX%Aux.SoundEffects.Count];					
		SFXPlayers[SFXPlayer%SFXPlayers.Count].Play();
	}
	public void StopSFX()
	{
		Tween AudioTween  = CreateTween();
		AudioTween.SetParallel(true);
		for(int i = 0; i < SFXPlayers.Count; i++)
		{
			AudioTween.TweenProperty(SFXPlayers[i],"volume_db",-80,0.3);
		}
		AudioTween.Finished += () =>
		{
			for(int i = 0; i < SFXPlayers.Count; i++)
			{
				SFXPlayers[i].Stop();			
			}
			AudioTween.Kill();			
		};
		
	}
}

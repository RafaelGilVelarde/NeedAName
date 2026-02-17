using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class SFXList : Resource
{
    [Export] public Array<AudioStream> SoundEffects;
}

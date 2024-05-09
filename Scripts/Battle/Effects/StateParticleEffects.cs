using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class StateParticleEffects : GpuParticles2D
{
    [Export]Array<Texture2D>ColorRamps;
    [Export] ParticleProcessMaterial processMaterial;
    public void EmitParticles(BattleCharacter.BattleState state){
        processMaterial.ColorRamp=ColorRamps[(int)state];
        Emitting=true;
    }
}

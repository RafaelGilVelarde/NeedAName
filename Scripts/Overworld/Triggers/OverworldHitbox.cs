using Godot;
using System;
using System.Diagnostics;

public partial class OverworldHitbox : Area2D
{
    public enum HitType
    {
        Normal,
        Slash,
        Water,

    }
    [Signal]
	public delegate void _HitEventHandler(OverworldHurtbox Target);

    [Export] string HurtboxTag;
    [Export] protected ProjectileHit HitEffect;
    [Export] public HitType Type;

    public override void _Ready()
    {
        base._Ready();
        AreaEntered+=OnTriggerEnter;
        if (HitEffect != null)
        {
            _Hit+=HitEffect.HitEffect;            
        }

    }

    	public virtual void OnTriggerEnter(Area2D node){
            Debug.WriteLine("Hit: "+node);
            if (node.IsInGroup(HurtboxTag))
            {
                Debug.WriteLine("Hit: "+node+" with tag: "+HurtboxTag);
                OverworldHurtbox Hurtbox = (OverworldHurtbox)node;
                EmitSignal("_Hit",Hurtbox);
                Hurtbox.EmitSignal("_Hurt",this);
            }
        }
}

using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;

public partial class OverworldController : Node2D
{
	[Export] protected Sprite2D MainSprite;
	[Export] public float Speed, SpeedModifier, ModifierDecay, InvFrames, CollisionTime;
	[Export] protected float Friction;
	[Export] protected float MaxSpeed;

	[Export] public AnimationPlayer Animator, EffectAnimator;
	[Export] public AnimationTree AnimatorTree;
	[Export] public CharacterBody2D Parent;
	[Export] public CollisionShape2D OverworldCollider;

	[Export] public int AxisOffset;

	[Export] public Vector2 Axis = Vector2.Zero, DirModifier;
	[Export] protected bool Controllable = true, OffsetsSet, inBattle;
	[Export] public Vector2 FacingDirection { get; protected set; }

	[Export] public TileCheck Tile;
	[Export] public BattleCharacter BattleCharacter;

	[Export] protected OverworldHurtbox Hurtbox;
	public override void _EnterTree()
	{
		if(BattleCharacter.Character.NodeCharacter == null){
			BattleCharacter.Character.ResourceLocalToScene = true;
			BattleCharacter.Character.NodeCharacter = Parent;
		}
		if (AnimatorTree == null && Animator == null)
		{
			SetAnimators();
		}
		SetOffsets();


	}
    public override void _Ready()
    {
        base._Ready();
		Hurtbox._Hurt+=GetHit;
    }
	public void SetAnimators()
	{
		AnimationPlayer OverworldAnimator = (AnimationPlayer)BattleCharacter.Character.Base.OverworldAnimator.Instantiate<AnimationPlayer>().Duplicate();
		AnimationPlayer BattleAnimator = (AnimationPlayer)BattleCharacter.Character.Base.BattleAnimator.Instantiate<AnimationPlayer>().Duplicate();
		AddChild(OverworldAnimator);
		AnimatorTree = OverworldAnimator.GetChild<AnimationTree>(0);
		BattleCharacter.AddChild(BattleAnimator);
		BattleCharacter.AnimatorPlayer = BattleAnimator;
		BattleCharacter.AnimatorTree = BattleAnimator.GetChild<AnimationTree>(0);
	}
	public virtual void BattleStart()
	{
		inBattle = true;
		ProcessMode = ProcessModeEnum.Disabled;
		SetControllable(false);
		EnableHurtbox(false);
		//OverworldCollider.Disabled = true;
		Hide();
		BattleCharacter.AnimatorTree.Active = true;
		BattleCharacter.TurnOnBattle();
	}
	public virtual void BattleEnd()
	{
		inBattle = false;
	}
	public void SetOffsets()
	{
		if (!OffsetsSet)
		{
			MainSprite.Position += BattleCharacter.Character.Base.OverworldOffset;
			OffsetsSet = true;
		}

	}
	public Tween TweenMovement(Vector2 Position, float Duration,bool ChangeFacingDirection = true,int Transition = (int)Tween.TransitionType.Linear, int Ease = (int)Tween.EaseType.InOut)
	{
		SetControllable(false);
		EnableHurtbox(false);
		if (ChangeFacingDirection)
		{
			Vector2 Aux = (Position-Parent.GlobalPosition).Normalized();
			Axis = Aux;
		}
		Tween Move = CreateTween();
		Move.TweenProperty(Parent, "position", Position, Duration).SetTrans((Tween.TransitionType)Transition).SetEase((Tween.EaseType)Ease);
		return Move;
	}
	public void SetControllable(bool control)
	{
		if (!control)
		{
			Axis = Vector2.Zero;
			Parent.Velocity = Vector2.Zero;
			AnimatorTree.Set("parameters/conditions/Idle", true);
			AnimatorTree.Set("parameters/conditions/Walking", false);
		}
		Controllable = control;
		//OverworldCollider.Disabled = !control;
	}
	public virtual void SetLayers(int GraphicsLayer, Array<int> CollisionLayer, Array<int> CollisionMask)
	{
		Parent.ZIndex = GraphicsLayer;
		for (int j = 1; j <= 32; j++)
			{
				Parent.SetCollisionLayerValue(j, CollisionLayer.Contains(j));
				Parent.SetCollisionMaskValue(j, CollisionMask.Contains(j));
			}
	}
	public void GetHit(Node2D node)
	{
		EffectAnimator.Play("Blink");
		EnableHurtbox(false);
		SetControllable(false);

		SceneTreeTimer Timer1 = GetTree().CreateTimer(CollisionTime,true,true);
		SceneTreeTimer Timer2 = GetTree().CreateTimer(InvFrames,true,true);

		Timer1.Timeout += () =>
		{
			SetControllable(true);
		};

		Timer2.Timeout += () =>
		{
			EffectAnimator.Stop();
			if (Controllable)
			{
				EnableHurtbox(true);				
			}
		};
	}
	public void ApplyForce(Vector2 Direction, float force, float Decay)
	{
		DirModifier = Direction.Normalized()*force;
		ModifierDecay = Decay;
	}
	public void EnableHurtbox(bool enable)
	{
		Hurtbox.GetChild<CollisionShape2D>(0).SetDeferred("disabled",!enable);
	}
}

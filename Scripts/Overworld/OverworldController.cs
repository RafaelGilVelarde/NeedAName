using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class OverworldController : Node2D
{
	[Export] protected Sprite2D MainSprite;
    	[Export] public float Speed;
	[Export] protected float Friction;
	[Export] protected float MaxSpeed;
    
	[Export] public AnimationPlayer Animator;
	[Export] public AnimationTree AnimatorTree;
    	[Export] public CharacterBody2D Parent;
	[Export] public CollisionShape2D OverworldCollider;
	[Export] public Array<Vector2> AxisList;
	[Export] public Array<Vector2> PositionList, AuxPositionList;
    	[Export] public int AxisOffset;

	[Export]public  Vector2 Axis=Vector2.Zero, AxisAux;
	[Export] protected bool Controllable = true, OffsetsSet;
	public Vector2 FacingDirection{get;protected set;}
	[Export]protected Vector2I Coords;
    	[Export] public BattleCharacter BattleCharacter;
    public override void _EnterTree()
    {
        BattleCharacter.Character.NodeCharacter = Parent;
		if(AnimatorTree == null){
			SetAnimators();
		}
		SetOffsets();


    }
	public void SetAnimators(){
			AnimationPlayer OverworldAnimator=(AnimationPlayer)BattleCharacter.Character.Base.OverworldAnimator.Instantiate<AnimationPlayer>().Duplicate();
			AnimationPlayer BattleAnimator=(AnimationPlayer)BattleCharacter.Character.Base.BattleAnimator.Instantiate<AnimationPlayer>().Duplicate();
			AddChild(OverworldAnimator);
			AnimatorTree=OverworldAnimator.GetChild<AnimationTree>(0);
			BattleCharacter.AddChild(BattleAnimator);
			BattleCharacter.AnimatorPlayer=BattleAnimator;
			BattleCharacter.AnimatorTree=BattleAnimator.GetChild<AnimationTree>(0);
	}
    public virtual void BattleStart(){
		ProcessMode=ProcessModeEnum.Disabled;
		OverworldCollider.Disabled=true;
		Hide();
		BattleCharacter.AnimatorTree.Active=true;
		BattleCharacter.TurnOnBattle();
	}
	public virtual void BattleEnd(){

	}
	public void SetOffsets(){
		if(!OffsetsSet){
			MainSprite.Position += BattleCharacter.Character.Base.OverworldOffset;
			OffsetsSet = true;
		}
		
	}
	public void TweenMovement(Vector2 Position, float Duration){
		SetControllable	(false);
		Tween Move = CreateTween();
		Move.TweenProperty(Parent,"position",Position,Duration);
	}
	public void SetControllable(bool control){
		if(!control){
			Axis=Vector2.Zero;
			AnimatorTree.Set("parameters/conditions/Idle",true);
			AnimatorTree.Set("parameters/conditions/Walking",false);
		}
		Controllable = control;
		OverworldCollider.Disabled = !control;
	}
}

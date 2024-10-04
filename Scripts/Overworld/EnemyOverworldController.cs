using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;


public enum MoveState{
	Idle,
	Seek,
	Run
}
public partial class EnemyOverworldController : OverworldController
{
	[Export] EnemyCollision enemyCollision;
	[Export] MovementController movementController;
	[Export] public Vector2 CachedTargetPosition, RandMin, RandMax;
	EnemyCharacterBase characterBase;
	[Export] public MoveState state;
	[Export] public Node2D CurrentTarget;
	[Export] public Array<Node2D> IdleTargets;
	[Export] public int CurrentIdleTarget;
	[Export] public Array<Node2D> Obstacles;
	[Export] public uint LayerMask;
	[Export] public DetectArea detectArea{get;private set;}
	[Export] public BattleStart Battle;
	public Vector2[] RaycastPos=new Vector2[8];
	public Vector2 RaycastTarget;
	[Export]public int DirectionIndex;
    public override void _Ready()
    {
		characterBase=(EnemyCharacterBase)BattleCharacter.Character.Base;
		CurrentTarget=GameManager.Instance.controller.Parent;
		if(detectArea!=null){
			(RandMin, RandMax) = MinMaxArray(detectArea.Polygon.Polygon);
		}
		CachedTargetPosition = GlobalPosition;


		if(AnimatorTree == null){
			SetAnimators();
		}
    }
    public override void _Process(double delta)
	{
		PlayAnimations(Axis);
		if(Axis!=Vector2.Zero){
			FacingDirection=Axis;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		if(movementController!=null){
			Vector2 Moving;				
			(Moving,Axis,DirectionIndex)=movementController.Moving(this);
			Vector2 Vel=Moving*(float)delta;
			Parent.Velocity+=Vel;
			Parent.Velocity=Parent.Velocity.LimitLength(MaxSpeed);
			Parent.MoveAndSlide();
		}
	}

	void PlayAnimations(Vector2 Axis){
		string Direction="Back";
		AnimatorTree.Set("parameters/Idle/blend_position",new Vector2(FacingDirection.X,-FacingDirection.Y));
		AnimatorTree.Set("parameters/Walking/blend_position",new Vector2(FacingDirection.X,-FacingDirection.Y));
		if(FacingDirection.X!=0){
			Direction="Side";
			Flip();
		}
		else if(FacingDirection.Y>0){
			Direction="Front";
		}
		else{
			Direction="Back";
		}
		if(Axis==Vector2.Zero){
			AnimatorTree.Set("parameters/conditions/Idle",true);
			AnimatorTree.Set("parameters/conditions/Walking",false);
			//Animator.Play("idle"+Direction);
		}
		else{
			AnimatorTree.Set("parameters/conditions/Idle",false);
			AnimatorTree.Set("parameters/conditions/Walking",true);
			//Animator.Play(Direction);
		}
	}
	void Flip(){
		if(FacingDirection.X/Mathf.Abs(FacingDirection.X)!=Parent.Scale.Y){
			Parent.Scale=new Vector2(Parent.Scale.X*-1,Parent.Scale.Y);
		}
	}

    (Vector2 Min, Vector2 Max) MinMaxArray(Vector2[] Array){
        Array<float> AuxX = new Array<float>(),AuxY = new Array<float>();
        for (int i = 0;i<Array.Length;i++){
            AuxX.Add(Array[i].X);
            AuxY.Add(Array[i].Y);
        }
        return (new Vector2(AuxX.Min(),AuxY.Min()),new Vector2(AuxX.Max(),AuxY.Max()));
        
    }

	public void SetArea(DetectArea area){
		detectArea = area;
		(RandMin, RandMax) = MinMaxArray(detectArea.Polygon.Polygon);
	}

	public override void BattleStart(){
		enemyCollision.ProcessMode=ProcessModeEnum.Disabled;
		base.BattleStart();
	}
    public override void BattleEnd()
    {
		ProcessMode=ProcessModeEnum.Inherit;
		OverworldCollider.Disabled=false;
		Show();
		enemyCollision.ProcessMode=ProcessModeEnum.Inherit;
    }
    public override void _Draw()
    {
					DrawLine(Transform.Origin,RaycastTarget,new Color(1,0,0));
    }
}

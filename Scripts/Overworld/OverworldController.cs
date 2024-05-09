using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class OverworldController : Node2D
{
	[Export] public float Speed;
	[Export] protected float Friction;
	[Export] protected float MaxSpeed;
	[Export] protected AnimationPlayer Animator;
	[Export] public AnimationTree AnimatorTree;
	[Export] public Area2D InteractCollider;
	[Export] Interact Interactable;
	[Export]public  Vector2 Axis=Vector2.Zero;
	protected Vector2 FacingDirection;
	[Export] protected RandomBattleStart random;
	[Export] public CharacterBody2D Parent;
	[Export] public BattleCharacter BattleCharacter;
	public bool Leader;
	[Export] public CollisionShape2D OverworldCollider;
	[Export] public OverworldController Controller;
	[Export] NavigationAgent2D Navigation;
	//[Export] Timer timer;
	[Export] public Array<Vector2> AxisList;
	[Export] public Array<Vector2> PositionList, AuxPositionList;
	[Export] public int AxisOffset;
	[Signal]public delegate void _FollowEventHandler(bool follow);

    public override void _Ready()
    {
        InteractCollider.BodyEntered+=OnInteractionEnter;
		InteractCollider.BodyExited+=OnInteractionExit;
		/*Navigation.PathDesiredDistance = 10.0f;
        Navigation.TargetDesiredDistance = 50.0f;*/
        Callable.From(ActorSetup).CallDeferred();	
    }
    public override void _Process(double delta)
    {
		PlayAnimations(Axis);
		if(Leader){
			Axis=GetInput();
			if(Input.IsActionJustPressed("Confirm")){
				if(Interactable!=null){
					Axis=Vector2.Zero;
					AnimatorTree.Set("parameters/conditions/Idle",true);
					AnimatorTree.Set("parameters/conditions/Walking",false);
					Interactable.interact();
				}
			}
			//if(Axis!=Vector2.Zero){
				RecordAxis();
			//}
		}
		/*else{
			if(Controller.Axis!=Vector2.Zero){
				FollowLeader();
			}
			else{
				Axis=Vector2.Zero;
			}
		}*/
		if(Axis!=Vector2.Zero){
			FacingDirection=Axis;
		}
    }
    public override void _PhysicsProcess(double delta)
	{
		/*if(!Leader && Controller!=null){
				if(!Navigation.IsNavigationFinished()){
					Axis=GlobalTransform.Origin.DirectionTo(Navigation.GetNextPathPosition()).Normalized();
				}
				else{
					Axis=Vector2.Zero;
					Debug.WriteLine("Reached");
				}
				Debug.WriteLine(Axis);
		}*/
		/*if(Leader){
			if(Axis!=Vector2.Zero){
				RecordAxis();
			}
		}
		else{
			if(Controller.Axis!=Vector2.Zero){
				FollowLeader();
			}
			else{
				Axis=Vector2.Zero;
			}			
		}*/
		if(Leader){
			Move(Speed * Axis * (float)delta);
		}
		if(Controller.Axis==Vector2.Zero){
				ApplyFriction((float)(Friction * delta));
		}
			Parent.MoveAndSlide();
	

		//Debug.WriteLine("Dir: "+FacingDirection.X/Mathf.Abs(FacingDirection.X));
		//Debug.WriteLine("Scale: "+Scale);

	}
	public Vector2 GetInput(){
		Vector2 Axis;
		Axis = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		return Axis.Normalized();	
	}
	public void Move(Vector2 accel){
		Parent.Velocity=accel;
	}
	public void ApplyFriction(float amount){
		if (Parent.Velocity.Length()>amount){
			Parent.Velocity-=Parent.Velocity.Normalized()*amount;
		}
		else{
			Parent.Velocity=Vector2.Zero;
			Parent.Velocity.LimitLength(MaxSpeed);
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
		if(Leader){
			if(Parent.Velocity==Vector2.Zero){
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
	}
	void Flip(){
		if(FacingDirection.X/Mathf.Abs(FacingDirection.X)!=Parent.Scale.Y){
			Parent.Scale=new Vector2(Parent.Scale.X*-1,Parent.Scale.Y);
		}
	}
	public void Seek(string path, float offset){
		AnimatorTree.Set(path, offset);
	}

	public virtual void BattleStart(){
		ProcessMode=ProcessModeEnum.Disabled;
		OverworldCollider.Disabled=true;
		Hide();
		BattleCharacter.AnimatorTree.Active=true;
		Interactable=null;
		BattleCharacter.TurnOnBattle();
	}
	public virtual void BattleEnd(){
		ClearAxis();
		ProcessMode=ProcessModeEnum.Inherit;
		if(Leader){
			OverworldCollider.Disabled=false;
		}
		Show();
	}
	
	void OnInteractionEnter(Node2D body){
		Debug.WriteLine(body.GetGroups());
		if(body.IsInGroup("Interactable")){
	
			Interactable=body.GetChild<Interact>(0);
        }
	}
	void OnInteractionExit(Node2D body){
		if(body.IsInGroup("Interactable")){
			Interactable=null;
        }
	}

	    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        Controller = GameManager.Instance.controller;
		ClearAxis();
    }
	
	public void FollowLeader(bool follow){
		AnimatorTree.Set("parameters/conditions/Idle",!follow);
		AnimatorTree.Set("parameters/conditions/Walking",follow);
		if(follow){
			Axis=Controller.AxisList[AxisOffset];
			Parent.Position=Controller.PositionList[AxisOffset];
		}
		else{
			Axis=Vector2.Zero;
		}
		/*if(Controller.PositionList[PositionList.Count-2]==Controller.PositionList[PositionList.Count-1]){
			Axis=Vector2.Zero;
		}
		else{
			Axis=Controller.AxisList[AxisOffset];
		}*/
		//Parent.Velocity=Controller.VelocityList[AxisOffset];
	}
	void RecordAxis(){
		if(Axis!=Vector2.Zero&&PositionList[PositionList.Count-1]!=GlobalPosition){
			PositionList.RemoveAt(0);
			PositionList.Add(GlobalPosition);
			//AuxPositionList=PositionList;
			AxisList.RemoveAt(0);
			AxisList.Add(Axis);		
			EmitSignal("_Follow",true);
		}
		else{
			EmitSignal("_Follow",false);
		}
		
	}
	void ClearAxis(){
		for(int i=0;i<AxisList.Count;i++){
			Controller.AxisList[i]=Vector2.Zero;
			Controller.PositionList[i]=Controller.GlobalPosition;
		}
	}
}

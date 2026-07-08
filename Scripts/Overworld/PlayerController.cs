using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public enum TileTypes{
	Normal,
	Water
}
public partial class PlayerController : OverworldController
{



	[Export] public Area2D InteractCollider;
	[Export] Interact Interactable;
	
	[Export] public DataTileMap DataMap;
	[Export] protected TileTypes tileTypes, PreviousTileType;
	[Export] protected Node2D TileDetector, TileDetector2;
	[Export] protected Vector2 TileOffset;


	[Export] protected RandomBattleStart random;
	public bool Leader = false;
	[Export] public PlayerController Controller;

	[Signal]public delegate void _FollowEventHandler(bool follow);

	
	[Signal]public delegate void _ConfirmEventHandler();
	[Signal]public delegate void _DenyEventHandler();
	[Signal]public delegate void _SwitchEventHandler();
	[Signal]public delegate void _MenuEventHandler();


	GameManager Game;


    public override void _Ready()
    {
        InteractCollider.BodyEntered+=OnInteractionEnter;
		InteractCollider.BodyExited+=OnInteractionExit;

		Game = GameManager.Instance;
        Callable.From(ActorSetup).CallDeferred();	
    }
    public override void _Input(InputEvent @event)
    {
        if(Controllable){
			if(Leader){
				if(Input.IsActionJustPressed("Confirm")){
					if(Interactable!=null){		
						Axis=Vector2.Zero;
						AnimatorTree.Set("parameters/conditions/Idle",true);
						AnimatorTree.Set("parameters/conditions/Walking",false);
						Interactable.interact(this);
					}
				}
				/*if(Input.IsActionJustPressed("Switch")){
					if (Game.Characters.Count > 1)
					{
						int index = Game.Characters.IndexOf(this);
						Game.ChangeLeader(this,Game.Characters[(index + 1) % Game.Characters.Count]);

						SceneTreeTimer SwitchTimer = GetTree().CreateTimer(0.1,true,true);
						SwitchTimer.Timeout+= () =>
						{
							Controller.SetControllable(true);							
						};
					}
				}
				if(Input.IsActionJustPressed("Deny")){
					Debug.WriteLine("Move: "+BattleCharacter.Character.FieldMove);
					if (BattleCharacter.Character.FieldMove != null)
					{
						BattleCharacter.Character.FieldMove.Base.Effect(this);						
					}
				}*/
				if(Input.IsActionJustPressed("Menu")){
					MainMenu.Instance.OpenCloseMenu(true);
					SetControllable(false);
				}
			}
		}
    }
    public override void _Process(double delta)
    {
		PlayAnimations(Axis);
			if(Axis!=Vector2.Zero && Axis.X*Axis.Y == 0){
				FacingDirection=Axis;
			}
		if(Controllable){
			AxisAux = Axis;
			if(Axis.X*Axis.Y!=0){
				AxisAux = new Vector2(Axis.X,0);
			}
			TileData Data = null;
			Vector2 SceneCoords = new Vector2();
			if (DataMap != null)
			{
				CurrentMapLayer = DataMap.Map[ZIndex%DataMap.Map.Count];
				PrevCoords = Coords;
				Vector2I BaseCoords = CurrentMapLayer.LocalToMap(Parent.GlobalPosition);
				Vector2I CoordAux = BaseCoords - Coords;

				if(CoordAux.X!= FacingDirection.X && CoordAux.Y!= FacingDirection.Y)
				{
					Coords = BaseCoords;
				}
				else
				{
					Coords = CurrentMapLayer.LocalToMap(Parent.GlobalPosition - (AxisAux * TileOffset));
				}

				Data = CurrentMapLayer.GetCellTileData(Coords);
				SceneCoords = CurrentMapLayer.MapToLocal(Coords);
			}
			if(Data!=null){
				PreviousTileType = tileTypes;
				tileTypes = (TileTypes)(int)Data.GetCustomData("TileType");
			}
			if (Leader)
			{
				switch (tileTypes)
				{
					case TileTypes.Normal:
						Axis = GetInput();
						break;
					case TileTypes.Water:
						if (tileTypes != PreviousTileType)
						{
							Parent.Position = SceneCoords;
						}
						if (DataMap != null)
						{
							TileData TileAux = DataMap.Map[ZIndex%DataMap.Map.Count].GetCellTileData(Coords);
							if (TileAux != null)
							{
								Axis = (Vector2)TileAux.GetCustomData("Direction");															
							}
						}
						break;
				}		
				RecordAxis();	
			}
			//Coords = DataMap.LocalToMap(TileDetector.GlobalPosition);
		}
    }
    public override void _PhysicsProcess(double delta)
	{
		if (Controllable)
		{
			if(Leader){
				Move(Speed * Axis * (float)delta);
			}
			if(Controller?.Axis==Vector2.Zero){
					ApplyFriction((float)(Friction * delta));
			}
		}
		Parent.MoveAndSlide();			
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
		Vector2 Direction = new Vector2(FacingDirection.X,-FacingDirection.Y);
		bool AuxDir = false;
		if ((Vector2)AnimatorTree.Get("parameters/Idle/blend_position") != Direction)
		{
			AuxDir = true;
		}
		AnimatorTree.Set("parameters/Idle/blend_position",Direction);
		AnimatorTree.Set("parameters/Walking/blend_position",Direction);

		if (AuxDir){
			AnimationNodeStateMachinePlayback Playback = (AnimationNodeStateMachinePlayback)AnimatorTree.Get("parameters/playback");
			Playback.Travel(Playback.GetCurrentNode());
		}
		
		if(FacingDirection.X!=0){
			Flip();
		}
		if(Leader){
			if(Parent.Velocity==Vector2.Zero){
					AnimatorTree.Set("parameters/conditions/Idle",true);
					AnimatorTree.Set("parameters/conditions/Walking",false);
			}
			else{
				AnimatorTree.Set("parameters/conditions/Idle",false);
				AnimatorTree.Set("parameters/conditions/Walking",true);
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

	public override void BattleStart(){
		Interactable=null;
		base.BattleStart();
		}
	public override void BattleEnd(){
		base.BattleEnd();
		ClearAxis();
		ProcessMode=ProcessModeEnum.Inherit;
		if(Leader){
			SetControllable(true);
		}
		Show();
	}
	
	void OnInteractionEnter(Node2D body){
		if(body.IsInGroup("Interactable")){
			foreach(Node child in body.GetChildren())
			{
				if(typeof(Interact).IsAssignableFrom(child.GetType())){
					Interactable=(Interact)child;				
					Debug.WriteLine("In Body: "+body);
				}
					Debug.WriteLine("Shape: "+child.Name);					
			}
        }
	}
	void OnInteractionExit(Node2D body){
		if(body.IsInGroup("Interactable")){
			Debug.WriteLine("Body: "+body);
			Interactable=null;
        }
	}

	private async void ActorSetup()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        Controller = GameManager.Instance.controller;
		ClearAxis();
    }
	
	public void FollowLeader(bool follow){
		AnimatorTree.Set("parameters/conditions/Idle",!follow);
		AnimatorTree.Set("parameters/conditions/Walking",follow);
		if (follow)
		{
			Axis = Game.FollowerLists.AxisList[AxisOffset];
			Parent.Position = Game.FollowerLists.PositionList[AxisOffset];
			Parent.ZIndex = Game.FollowerLists.ZIndexList[AxisOffset];
			Parent.CollisionLayer = Game.FollowerLists.CLayerList[AxisOffset];
			Parent.CollisionMask = Game.FollowerLists.CMaskList[AxisOffset];
		}
		else
		{
			Axis = Vector2.Zero;
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
		if(Axis!=Vector2.Zero&& Game.FollowerLists.PositionList[Game.FollowerLists.PositionList.Count-1]!=GlobalPosition){
			Game.FollowerLists.PositionList.RemoveAt(0);
			Game.FollowerLists.PositionList.Add(GlobalPosition);
			//AuxPositionList=PositionList;
			Game.FollowerLists.AxisList.RemoveAt(0);
			Game.FollowerLists.AxisList.Add(Axis);
			RecordLayers();
			EmitSignal("_Follow",true);
		}
		else{
			EmitSignal("_Follow",false);
		}
	}
	void RecordLayers()
	{
		Game.FollowerLists.CLayerList.RemoveAt(0);
		Game.FollowerLists.CLayerList.Add(Parent.CollisionLayer);
		Game.FollowerLists.CMaskList.RemoveAt(0);
		Game.FollowerLists.CMaskList.Add(Parent.CollisionMask);
		Game.FollowerLists.ZIndexList.RemoveAt(0);
		Game.FollowerLists.ZIndexList.Add(Parent.ZIndex);
	}
	void ClearAxis(){
		if(Controller!=null){
			for(int i=0;i<Game.FollowerLists.AxisList.Count;i++){
				Controller.Game.FollowerLists.AxisList[i]=Vector2.Zero;
				Controller.Game.FollowerLists.PositionList[i]=Controller.GlobalPosition;
			}
		}
	}



	public void EnterExitDialogue(bool Enter){
		if (!inBattle)
		{
			SetControllable(!Enter);			
		}
	}


}

using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public partial class GameManager : Node
{
	[Export] public string SavePath;
	[Export]public Array<DataManager> Saves;
	[Export] public int CurrentSave = 0;
	[Export]public DataManager Data;
	[Export] public Array<Maps> AreaMaps;
	[Export] PackedScene[] CharacterPrefabs;
	[Export] public PackedScene[] TextEffectPrefabs;
	[Export] public Array<PlayerController> Characters, Followers = new Array<PlayerController>();
	[Export] public Camera2D OverworldCam, BattleCam;
	[Export] PackedScene StartScene;
	[Export] AnimationPlayer TransitionAnimator;
	[Export] TextureRect TransitionOverlay;
	[Export] Color TransitionColor;
	[Export] float TransitionTime = 0.4f;
	public static GameManager Instance;

	[Export] public PlayerController controller;

	Vector2 PositionToMove;
	public Tween TransitionTween;
	


	public override void _Ready()
	{
	}
	public override void _EnterTree()
	{
		Instance=this;
		//InstantiateCharacters();
		 ResourceLoader.Exists("user://"+"save"+0.ToString()+".tres");
		 if(GetTree().CurrentScene.Name!="MainMenu"){
			InstantiateCharacters();
		 }
	}
	public void LoadFirstScene(){
		InstantiateCharacters();
		CallDeferred("SwitchScene",0,0, 0);
	}
	void InstantiateCharacters(){
		for(int i=0;i<Data.Party.Count;i++){
			Character aux=Data.Party[i];
			if(aux.Active){
				Characters.Add((PlayerController)AddCharacters(aux,0));
				if(i>0){
					Characters[i].AxisOffset=i*3;
				}
			}
		}
		for(int i=0;i<Data.CurrentFollowers.Count;i++){
			Character aux=Data.CurrentFollowers[i];
			if(aux.Active){
				Followers.Add((PlayerController)AddCharacters(aux,0));
				if(i>0){
					Followers[i].AxisOffset=i*3;
				}
			}
		}
		SetLayers(Data.GraphicsLayer,Data.PhysicsLayer);
		ChangeLeader(Characters[0],Characters[0]);
	}
	public void AddFollowingCharacter(PlayerController A, bool NPC){
		A.OverworldCollider.Disabled = true;
		Tween tween = CreateTween();
		tween.TweenProperty(A.Parent,"position",controller.GlobalPosition,0.2f);
		tween.Finished+=()=>{
			if(NPC){
				Followers.Add(A);
				A.AxisOffset = (Followers.Count-1)*3;
			}
			else{
				A.AxisOffset = (Data.Party.IndexOf((PartyCharacters)A.BattleCharacter.Character)-1)*3;
			}
			controller._Follow+=A.FollowLeader;
		};
		tween.Finished+=tween.Kill;
		//A.Parent.Position = controller.GlobalPosition;
	}
	public void ChangeLeader(PlayerController A, PlayerController B){
		A.Leader=false;
		B.Leader=true;
		A.OverworldCollider.Disabled=false;
		controller=B;
		for(int i=0;i<Characters.Count;i++){
			if(Characters[i]!=A){
				A._Follow-=Characters[i].FollowLeader;
			}
			if(Characters[i]!=B){
				B._Follow+=Characters[i].FollowLeader;
			}
		}
		for(int i=0;i<Followers.Count;i++){
			if(Followers[i]!=A){
				A._Follow-=Followers[i].FollowLeader;
			}
			if(Characters[i]!=B){
				B._Follow+=Followers[i].FollowLeader;
			}
		}
	}

	public void BattleStart(){
		for(int i=0;i<Characters.Count;i++){
			Characters[i].BattleStart();
		}
	}
	public void ChangeCam(Vector2 CamPosition, bool BattleStart,float duration){
		Tween tween=CreateTween();
		Camera2D cam;
		if(BattleStart){
			BattleCam.PositionSmoothingEnabled=false;
			BattleCam.Position=OverworldCam.GetScreenCenterPosition();
			cam=BattleCam;
			OverworldCam.Enabled=false;
			BattleCam.Enabled=true;			
			BattleCam.PositionSmoothingEnabled=true;
		}
		else{
			OverworldCam.PositionSmoothingEnabled=false;
			OverworldCam.GlobalPosition=BattleCam.GetScreenCenterPosition();
			cam=OverworldCam;
			BattleCam.Enabled=false;
			OverworldCam.Enabled=true;	
			OverworldCam.PositionSmoothingEnabled=true;
		}
		tween.TweenProperty(cam,"position",CamPosition,duration);
		tween.Finished+=tween.Kill;
		
	}
	void AssignCharacterCamera(Node2D Character){
		OverworldCam.GetParent().RemoveChild(OverworldCam);
		Character.AddChild(OverworldCam);
	}
	void AssignBattleCamera(Node Scene){
		BattleCam.GetParent().RemoveChild(BattleCam);
		Scene.AddChild(BattleCam);
	}
	public OverworldController AddCharacters(Character character, int  prefab){
			OverworldController Overworld=new OverworldController();
			Node Prefab=CharacterPrefabs[prefab].Instantiate<Node>();
			Overworld=Prefab.GetNode<OverworldController>("./OverworldController");
			Overworld.BattleCharacter.Character= character;
			character.SetTotalStats();
			Overworld.SetAnimators();

			Overworld.SetOffsets();
			GetTree().CurrentScene.AddChild(Prefab);
			return Overworld;
	}
	public void SetCamera(Camera2D cam){
		OverworldCam=cam;
		AssignCharacterCamera(controller);                
	}
	public void SetBattleCamera(Camera2D cam){
		BattleCam=cam;
		AssignBattleCamera(GetTree().CurrentScene);                
	}
	public void SwitchScene(int scene, int Area, Vector2 Position){

		PlayTransition(TransitionColor);
		//SceneTreeTimer timer = GetTree().CreateTimer(0.6f,true,true,true);
		/*timer.Timeout+=()=>*/
		TransitionTween.Finished+=()=>
		{
			Data.Scene = scene;
			Data.AreaIndex = Area;
			RootCharacters();
			OverworldCam=null;
			BattleCam=null;
			controller?.SetControllable(false);
			PositionToMove = Position;
			GetTree().ChangeSceneToPacked(AreaMaps[Area].maps[scene]);};
		/*SceneTreeTimer timer = GetTree().CreateTimer(0.5f,true,true,true);
		timer.Timeout+=()=>MoveCharactersToScene(Position);*/
	}
	public void RootCharacters(){
		//controller.RemoveChild(OverworldCam);
		//GetTree().CurrentScene.AddChild(OverworldCam);
		if(IsInstanceValid(OverworldCam)){
			OverworldCam?.QueueFree();
		}
		if(IsInstanceValid(BattleCam)){
			BattleCam?.QueueFree();
		}
		for(int i=0;i<Characters.Count;i++){
			Node2D aux=Characters[i].Parent;
			aux.Reparent(GetTree().Root);
			//aux.GetParent().RemoveChild(aux);
			aux.Position = new Vector2(int.MinValue,int.MinValue);
			
			//GetTree().Root.AddChild(aux);
		}
		for(int i=0;i<Followers.Count;i++){
			Node2D aux=Followers[i].Parent;
			aux.GetParent().RemoveChild(aux);
			aux.Position = new Vector2(int.MinValue,int.MinValue);
			GetTree().Root.AddChild(aux);
		}
	}
	public void SetDataTileMap(TileMap Map){
		for(int i=0;i<Characters.Count;i++){
			Characters[i].DataMap=Map;
		}		
		for(int i=0;i<Followers.Count;i++){
			Followers[i].DataMap=Map;
		}	
	}
	public void MoveCharactersToScene(){
		for(int i=0;i<Characters.Count;i++){
			Node2D aux=Characters[i].Parent;
			aux.GlobalPosition=PositionToMove;
			aux.GetParent().RemoveChild(aux);
			GetTree().CurrentScene.AddChild(aux);
		}
		for(int i=0;i<Followers.Count;i++){
			Node2D aux=Followers[i].Parent;
			aux.GlobalPosition=PositionToMove;
			aux.GetParent().RemoveChild(aux);
			GetTree().CurrentScene.AddChild(aux);
		}	
	}
	public void Save(int save){
		CurrentSave = save;
		string Path = "user://"+"save"+CurrentSave.ToString()+".tres";

		Data.Position = controller.GlobalPosition;
		Saves[CurrentSave]= Data.DuplicateData();
		if(!Godot.FileAccess.FileExists(Path)){
			Godot.FileAccess file = Godot.FileAccess.Open(Path,Godot.FileAccess.ModeFlags.WriteRead);
		}
		ResourceSaver.Save(Saves[CurrentSave],Path);
		Saves[CurrentSave].TakeOverPath(Path);
	}
	public void DisplaySaves(){
		for(int i=0;i<Saves.Count;i++){
		string Path = "user://"+"save"+i.ToString()+".tres";
		Saves[i] = null;
		if(ResourceLoader.Exists(Path)){
			Saves[i]= (DataManager)ResourceLoader.Load<DataManager>(Path,null,ResourceLoader.CacheMode.Replace).Duplicate(true);
		}
		else{
			Saves[i] = null;
		}
		}
	}
	public void Load(int Save){
		for(int i = 0;i<Characters.Count;i++){
			Characters[i].Parent.QueueFree();
		}
		Data = null;
		Data= Saves[Save].DuplicateData();
		CurrentSave=Save;
		OverworldCam?.QueueFree();
		BattleCam?.QueueFree();
		Characters.Clear();
		Followers.Clear();

		InstantiateCharacters();
		CallDeferred("SwitchScene",Data.Scene, Data.AreaIndex, Data.Position);
	}
	
	public void DeleteSave(int Save){
		Saves.RemoveAt(Save);
	}
	public void Restart(){
		Characters.Clear();
		Followers.Clear();
		Data = null;
		OverworldCam = null;
		BattleCam = null;
		GetTree().ChangeSceneToPacked(StartScene);
	}
	public void PlayTransition(Color color){
		TransitionTween = CreateTween();
		TransitionTween.TweenProperty(TransitionAnimator.GetParent<Control>(),"modulate",color,TransitionTime);
		TransitionTween.Finished+=TransitionTween.Kill;
		/*TransitionAnimator.Stop();
		if(TransitionAnimator.AssignedAnimation!=null){
			TransitionAnimator.AssignedAnimation = null;
		}
		TransitionAnimator.Play("Transition");*/
	}
	public void SetLayers(int GraphicsLayer, uint PhysicsLayer){
		Data.GraphicsLayer = GraphicsLayer;
		Data.PhysicsLayer = PhysicsLayer;
		for(int i=0;i<Characters.Count;i++){
			Characters[i].Parent.ZIndex = GraphicsLayer;
			Characters[i].Parent.CollisionMask = PhysicsLayer;
		}
		for(int i=0;i<Followers.Count;i++){
			Followers[i].Parent.ZIndex = GraphicsLayer;
			Followers[i].Parent.CollisionMask = PhysicsLayer;			
		}	
	}
}

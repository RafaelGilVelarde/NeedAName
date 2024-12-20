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
	[Export] public Array<OverworldController> Characters;
	[Export] public Camera2D OverworldCam, BattleCam;
	[Export] PackedScene StartScene;
	public static GameManager Instance;

	[Export] public OverworldController controller;

	Vector2 PositionToMove;


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
				Characters.Add(AddCharacters(aux,0));
				if(i>0){
					Characters[i].AxisOffset=i*3;
				}
			}
		}
		ChangeLeader(Characters[0],Characters[0]);
	}
	public void ChangeLeader(OverworldController A, OverworldController B){
		A.Leader=false;
		B.Leader=true;
		A.OverworldCollider.Disabled=false;
		controller=A;
		for(int i=0;i<Characters.Count;i++){
			if(Characters[i]!=B){
				B._Follow+=Characters[i].FollowLeader;
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
		Data.Scene = scene;
		Data.AreaIndex = Area;
		RootCharacters();
		OverworldCam=null;
		BattleCam=null;
		GetTree().ChangeSceneToPacked(AreaMaps[Area].maps[scene]);
		PositionToMove = Position;
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
			aux.GetParent().RemoveChild(aux);
			aux.Position = new Vector2(int.MinValue,int.MinValue);
			GetTree().Root.AddChild(aux);
		}
	}
	public void SetDataTileMap(TileMap Map){
		for(int i=0;i<Characters.Count;i++){
			Characters[i].DataMap=Map;
		}		
	}
	public void MoveCharactersToScene(){
		for(int i=0;i<Characters.Count;i++){
			Node2D aux=Characters[i].Parent;
			aux.GlobalPosition=PositionToMove;
			aux.GetParent().RemoveChild(aux);
			GetTree().CurrentScene.AddChild(aux);
			/*if(GetTree().CurrentScene!=null){
				Debug.WriteLine("Character: "+aux+" Scene: "+GetTree().CurrentScene);
			}
			else{
				SceneTreeTimer timer = GetTree().CreateTimer(0.5f,true,true,true);
				timer.Timeout+=()=>GetTree().CurrentScene.AddChild(aux);
			}*/
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
			Debug.WriteLine("SaveScene: "+Saves[i]?.Party[0].stats.HP);
			Saves[i]= (DataManager)ResourceLoader.Load<DataManager>(Path,null,ResourceLoader.CacheMode.Replace).Duplicate(true);
			Debug.WriteLine("SaveScene: "+Saves[i].Party[0].stats.HP);
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

		InstantiateCharacters();
		CallDeferred("SwitchScene",Data.Scene, Data.AreaIndex, Data.Position);
	}
	
	public void DeleteSave(int Save){
		Saves.RemoveAt(Save);
	}
	public void Restart(){
		Characters.Clear();
		Data = null;
		OverworldCam = null;
		BattleCam = null;
		GetTree().ChangeSceneToPacked(StartScene);
	}
}

using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class GameManager : Node
{
	[Export] public string SavePath;
	[Export]public Array<DataManager> Saves;
	[Export] public int CurrentSave;
	[Export]public DataManager Data;
	[Export] public Array<Maps> AreaMaps;
	[Export] PackedScene[] CharacterPrefabs;
	[Export] public PackedScene[] TextEffectPrefabs;
	[Export] public Array<OverworldController> Characters;
	[Export] public Camera2D OverworldCam, BattleCam;
	public static GameManager Instance;

	[Export] public OverworldController controller;


	public override void _Ready()
	{
	}
	public override void _EnterTree()
	{
		Instance=this;
		InstantiateCharacters();
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
		tween.Finished+=()=>Debug.WriteLine("endedcammovement");
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
			Overworld.BattleCharacter.Character= (Character)character.Duplicate();

			/*AnimationPlayer OverworldAnimator=character.Base.OverworldAnimator.Instantiate<AnimationPlayer>();
			AnimationPlayer BattleAnimator=character.Base.BattleAnimator.Instantiate<AnimationPlayer>();
			Overworld.AddChild(OverworldAnimator);
			Overworld.AnimatorTree=OverworldAnimator.GetChild<AnimationTree>(0);
			Overworld.BattleCharacter.AddChild(BattleAnimator);
			Overworld.BattleCharacter.AnimatorPlayer=BattleAnimator;
			Overworld.BattleCharacter.AnimatorTree=BattleAnimator.GetChild<AnimationTree>(0);*/
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
		Debug.WriteLine("AA");
		RootCharacters();
		OverworldCam=null;
		BattleCam=null;
		GetTree().ChangeSceneToPacked(AreaMaps[Area].maps[scene]);
		SceneTreeTimer timer = GetTree().CreateTimer(0.05f,true,true,true);
		timer.Timeout+=()=>MoveCharactersToScene(Position);
	}
	public void RootCharacters(){
		//controller.RemoveChild(OverworldCam);
		//GetTree().CurrentScene.AddChild(OverworldCam);
		OverworldCam.QueueFree();
		BattleCam.QueueFree();
		for(int i=0;i<Characters.Count;i++){
			Node2D aux=Characters[i].Parent;
			aux.GetParent().RemoveChild(aux);
			GetTree().Root.AddChild(aux);
		}
	}
	public void SetDataTileMap(TileMap Map){
		for(int i=0;i<Characters.Count;i++){
			Characters[i].DataMap=Map;
		}		
	}
	public void MoveCharactersToScene(Vector2 Position){
		for(int i=0;i<Characters.Count;i++){
			Node2D aux=Characters[i].Parent;
			aux.GlobalPosition=Position;
			aux.GetParent().RemoveChild(aux);
			GetTree().CurrentScene.AddChild(aux);
		}	
	}
	public void Save(){
		Saves[CurrentSave]=Data;
		ResourceSaver.Save(Saves[CurrentSave],SavePath+"/"+CurrentSave+"/save.tres");
	}
	public void DisplaySaves(){
		for(int i=0;i<Saves.Count;i++){
			Saves[i]=ResourceLoader.Load<DataManager>(SavePath+"/"+i+"/save.tres");
		}
	}
	public void Load(int Save){
		Data=Saves[Save];
		CurrentSave=Save;
		InstantiateCharacters();
		SwitchScene(Data.Scene, Data.AreaIndex,Data.Position);
	}
}

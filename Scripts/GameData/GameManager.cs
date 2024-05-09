using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class GameManager : Node
{
	[Export]public DataManager Data;
	[Export] PackedScene[] CharacterPrefabs;
	[Export] public PackedScene[] TextEffectPrefabs;
	[Export] public Array<OverworldController> Characters;
	[Export] public Camera2D OverworldCam, BattleCam;
	public static GameManager Instance;

	public OverworldController controller;


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
			/*Node Prefab=CharacterPrefab.Instantiate<Node>();
			GetTree().CurrentScene.AddChild(Prefab);
			Characters.Add(Prefab.GetNode<OverworldController>("./OverworldController"));
			OverworldController Aux=Characters[Characters.Count-1];         
			Aux.BattleCharacter.Character=Data.Party[i];

			AnimationPlayer OverworldAnimator=Data.Party[i].Base.OverworldAnimator.Instantiate<AnimationPlayer>();
			AnimationPlayer BattleAnimator=Data.Party[i].Base.BattleAnimator.Instantiate<AnimationPlayer>();

			Aux.AddChild(OverworldAnimator);
			Aux.AnimatorTree=OverworldAnimator.GetChild<AnimationTree>(0);
			Aux.BattleCharacter.AddChild(BattleAnimator);
			Aux.BattleCharacter.AnimatorTree=BattleAnimator.GetChild<AnimationTree>(0);
			AssignCharacterCamera((Node2D)Prefab);*/
			Character aux=Data.Party[i];
			if(aux.Active){
				Characters.Add(AddCharacters(aux,0));
				if(i>0){
						//Characters[i].GetParent<Node2D>().GlobalPosition=Characters[i-1].GlobalPosition-Vector2.Down*10;
					Characters[i].AxisOffset=i*3;
				}
			}
		}
		Debug.WriteLine(Characters.Count);
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
			/*if(Characters[i]!=A){
				A._Follow-=Characters[i].FollowLeader;
			}*/
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
			Debug.WriteLine("BattlecamPos: "+BattleCam.GlobalPosition);
			cam=BattleCam;
			OverworldCam.Enabled=false;
			BattleCam.Enabled=true;			
			BattleCam.PositionSmoothingEnabled=true;
		}
		else{
			OverworldCam.PositionSmoothingEnabled=false;
			OverworldCam.GlobalPosition=BattleCam.GetScreenCenterPosition();
			Debug.WriteLine("OverworldcamPos: "+OverworldCam.GlobalPosition);
			Debug.WriteLine("BattlecamTarget: "+BattleCam.GetTargetPosition());
			Debug.WriteLine("BattlecamPos: "+BattleCam.GetScreenCenterPosition());
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
		Debug.WriteLine(prefab);
			OverworldController Overworld=new OverworldController();
			Node Prefab=CharacterPrefabs[prefab].Instantiate<Node>();
			Debug.WriteLine("Prefab: "+Prefab);
			Debug.WriteLine("Scene: "+GetTree().CurrentScene);
			GetTree().CurrentScene.AddChild(Prefab);
			Overworld=Prefab.GetNode<OverworldController>("./OverworldController");
			Overworld.BattleCharacter.Character= (Character)character.Duplicate();

			AnimationPlayer OverworldAnimator=character.Base.OverworldAnimator.Instantiate<AnimationPlayer>();
			AnimationPlayer BattleAnimator=character.Base.BattleAnimator.Instantiate<AnimationPlayer>();

			Overworld.AddChild(OverworldAnimator);
			Overworld.AnimatorTree=OverworldAnimator.GetChild<AnimationTree>(0);
			Overworld.BattleCharacter.AddChild(BattleAnimator);
			Overworld.BattleCharacter.AnimatorPlayer=BattleAnimator;
			Overworld.BattleCharacter.AnimatorTree=BattleAnimator.GetChild<AnimationTree>(0);


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
	public async void SwitchScene(PackedScene scene, Vector2 Position){
		RootCharacters();
		OverworldCam=null;
		BattleCam=null;
		GetTree().ChangeSceneToPacked(scene);
		await ToSignal(GetTree().CreateTimer(0.05f),"timeout");
		MoveCharactersToScene(Position);
	}
	public void RootCharacters(){
		controller.RemoveChild(OverworldCam);
		GetTree().CurrentScene.AddChild(OverworldCam);
		for(int i=0;i<Characters.Count;i++){
			Node2D aux=Characters[i].Parent;
			aux.GetParent().RemoveChild(aux);
			GetTree().Root.AddChild(aux);
		}
	}
	public void MoveCharactersToScene(Vector2 Position){
		for(int i=0;i<Characters.Count;i++){
			Node2D aux=Characters[i].Parent;
			aux.GetParent().RemoveChild(aux);
			GetTree().CurrentScene.AddChild(aux);
			aux.GlobalPosition=Position;
		}	
	}
}

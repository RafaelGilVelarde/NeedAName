using Godot;
using Godot.Collections;
using GodotPlugins.Game;
using MonoCustomResourceRegistry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

public partial class GameManager : Node
{
	[Export] public string SavePath;
	[Export] public Array<DataManager> Saves;
	[Export] public DataManager Data;
	[Export] public MainSettings Settings;
	[Export] public bool SavesExist, CurrentSaveExists;
	[Export] DataManager InitialData;
	[Export] public Array<Maps> AreaMaps;
	[Export] PackedScene[] CharacterPrefabs;
	[Export] public PackedScene[] TextEffectPrefabs;
	[Export] public Array<PlayerController> Characters, Followers = new Array<PlayerController>();
	[Export] public Camera OverworldCam, BattleCam;
	[Export] PackedScene StartScene;
	[Export] public Scene CurrentScene;
	[Export] AnimationPlayer TransitionAnimator;
	[Export] TextureRect TransitionOverlay;
	[Export] Color TransitionColor;
	[Export] float TransitionTime = 0.4f, ExpSpeed, ExpDistance;
	[Export] public AudioStreamPlayer MainAudio;
	[Export] public Array<AudioStream> BGMs;

	[Export] int CurrentBGM = -1;


	public static GameManager Instance;

	[Export] public PlayerController controller;


	[Signal] public delegate void _LoadEventHandler();

	Vector2 PositionToMove;
	public Tween TransitionTween;



	public override void _Ready()
	{
	}
	public override void _EnterTree()
	{
		Instance = this;
		//InitialData = (DataManager)Data.Duplicate();
		//InstantiateCharacters();
		ResourceLoader.Exists("user://" + "save" + 0.ToString() + ".tres");
		DisplaySaves();
		
		
		string SettingsPath = "user://" + "settings" + ".tres";
		if (ResourceLoader.Exists(SettingsPath))
        {
			Settings = (MainSettings)ResourceLoader.Load<MainSettings>(SettingsPath, null, ResourceLoader.CacheMode.Replace).Duplicate(true);
            if (Saves[Settings.CurrentSave] != null)
			{
				Debug.WriteLine("Exists");
				Data = Saves[Settings.CurrentSave];
            }
        }

		TranslationServer.SetLocale(Settings.Language);
		Debug.WriteLine(Saves.Count);

		for (int i = 0; i < InitialData.Party.Count; i++)
		{
			SetEXPLevels((PartyCharacterBase)InitialData.Party[i].Base);
			InitialData.Party[i].NextLevelExp = ((PartyCharacterBase)InitialData.Party[i].Base).ExpForLevel[InitialData.Party[i].stats.Lv - 1];
		}
		if (GetTree().CurrentScene.Name != "MainMenu")
		{
			InstantiateCharacters();
		}
	}
	public void LoadFirstScene()
	{
		Data = InitialData.DuplicateData();
		CurrentSaveExists = false;
		InstantiateCharacters();
		Array<int> InitColLayerArray = new Array<int>();
		Array<int> InitColMaskArray = new Array<int>();

		InitColLayerArray.Add(1);
		InitColMaskArray.Add(1); 
		CallDeferred("SwitchScene", 0, 0, Vector2.Zero, TransitionColor, 0,InitColLayerArray,InitColMaskArray);
	}
	void InstantiateCharacters()
	{
		for (int i = 0; i < Data.Party.Count; i++)
		{
			PartyCharacters aux = Data.Party[i];
			if (aux.Active)
			{
				Characters.Add((PlayerController)AddCharacters(aux, 0));
				Followers.Add(Characters[Characters.Count - 1]);
				int FollowerCount = Followers.Count - 1;

				Debug.WriteLine($"{i}: Party {((PartyCharacterBase)aux.Base).PartyId}");
				Debug.WriteLine(aux.Active);
				Characters[Characters.Count - 1].Parent.Name = $"Party {((PartyCharacterBase)aux.Base).PartyId}";
				Characters[Characters.Count - 1].InteractCollider.GetChild<CollisionShape2D>(0).Disabled = true;
				Debug.WriteLine($"Name {i}: {Characters[Characters.Count-1].Parent.Name}");
				if (i > 0)
				{
					Characters[Characters.Count - 1].AxisOffset = FollowerCount * 8;
				}
			}
		}
		for (int i = 0; i < Data.CurrentFollowers.Count; i++)
		{
			Character aux = Data.CurrentFollowers[i];
			if (aux.Active)
			{
				Followers.Add((PlayerController)AddCharacters(aux, 0));
				Followers[Followers.Count - 1].Parent.Name = $"Follower {i}";
				Followers[Followers.Count - 1].InteractCollider.GetChild<CollisionShape2D>(0).Disabled = true;
				int FollowerCount = Followers.Count - 1;
				if (i > 0)
				{
					Followers[Followers.Count - 1].AxisOffset = FollowerCount * 8;
				}
			}
		}
		for (int i = 0; i < Characters.Count; i++)
		{
			Characters[i].SetLayers(Data.GraphicsLayer, Data.CollisionLayer, Data.CollisionMask);
		}
		for (int i = 0; i < Followers.Count; i++)
		{
			Followers[i].SetLayers(Data.GraphicsLayer, Data.CollisionLayer, Data.CollisionMask);

		}
		ChangeLeader(Characters[0], Characters[0]);
	}
	public void AddFollowingCharacter(PlayerController A, bool NPC)
	{
		A.OverworldCollider.Disabled = true;
		Tween tween = CreateTween();
		tween.TweenProperty(A.Parent, "position", controller.GlobalPosition, 0.2f);
		tween.Finished += () =>
		{
			Followers.Add(A);
			A.AxisOffset = (Followers.Count - 1) * 3;
			if (!NPC)
			{
				Characters.Add(A);
				//A.AxisOffset = (Data.Party.IndexOf((PartyCharacters)A.BattleCharacter.Character)-1)*3;
				((PartyCharacters)A.BattleCharacter.Character).Active = true;
			}
			controller._Follow += A.FollowLeader;
		};
		tween.Finished += tween.Kill;
		//A.Parent.Position = controller.GlobalPosition;
	}
	public void RemoveFollowingCharacter(PlayerController A, bool NPC)
	{
		Debug.WriteLine("Removing");
		A.OverworldCollider.Disabled = true;
		Tween tween = CreateTween();
		tween.TweenProperty(A.Parent, "modulate:a", 0, 0.2f);
		tween.Finished += () =>
		{
			Followers.Remove(A);
			if (!NPC)
			{
				Characters.Remove(A);
				//A.AxisOffset = (Data.Party.IndexOf((PartyCharacters)A.BattleCharacter.Character)-1)*3;
				((PartyCharacters)A.BattleCharacter.Character).Active = false;
			}
			controller._Follow -= A.FollowLeader;
			A.Parent.QueueFree();
			Debug.WriteLine("Deleted");
		};
		tween.Finished += tween.Kill;
		//A.Parent.Position = controller.GlobalPosition;
	}
	public void ChangeLeader(PlayerController A, PlayerController B)
	{
		A.Leader = false;
		B.Leader = true;
		A.OverworldCollider.Disabled = true;
		B.OverworldCollider.Disabled = false;
		A.SetControllable(false);						
		controller = B;
		B.InteractCollider.GetChild<CollisionShape2D>(0).Disabled = false;
		for (int i = 0; i < Characters.Count; i++)
		{
			if (Characters[i] != A)
			{
				A._Follow -= Characters[i].FollowLeader;
			}
			if (Characters[i] != B)
			{
				B._Follow += Characters[i].FollowLeader;
			}
		}
		for (int i = 0; i < Followers.Count; i++)
		{
			Characters[i].Controller = B;
			if (Followers[i] != A)
			{
				A._Follow -= Followers[i].FollowLeader;
			}
			if (Characters[i] != B)
			{
				B._Follow += Followers[i].FollowLeader;
			}
		}

		B.Parent.Position = A.Parent.Position;
		
	}

	public void BattleStart()
	{
		for (int i = 0; i < Characters.Count; i++)
		{
			Characters[i].BattleStart();
		}
	}
	public void ChangeCam(Vector2 CamPosition, bool BattleStart, float duration)
	{
		Tween tween = CreateTween();
		Camera2D cam;
		if (BattleStart)
		{
			BattleCam.PositionSmoothingEnabled = false;
			BattleCam.Position = OverworldCam.GetScreenCenterPosition();
			cam = BattleCam;
			OverworldCam.Enabled = false;
			BattleCam.Enabled = true;
			BattleCam.PositionSmoothingEnabled = true;
		}
		else
		{
			OverworldCam.PositionSmoothingEnabled = false;
			OverworldCam.GlobalPosition = BattleCam.GetScreenCenterPosition();
			cam = OverworldCam;
			BattleCam.Enabled = false;
			OverworldCam.Enabled = true;
			OverworldCam.PositionSmoothingEnabled = true;
		}
		tween.TweenProperty(cam, "position", CamPosition, duration);
		tween.Finished += tween.Kill;

	}
	void AssignCharacterCamera(Node2D Character)
	{
		OverworldCam.GetParent().RemoveChild(OverworldCam);
		Character.AddChild(OverworldCam);
	}
	void AssignBattleCamera(Node Scene)
	{
		BattleCam.GetParent().RemoveChild(BattleCam);
		Scene.AddChild(BattleCam);
	}
	public OverworldController AddCharacters(Character character, int prefab)
	{
		OverworldController Overworld = new OverworldController();
		Node Prefab = CharacterPrefabs[prefab].Instantiate<Node>();
		Overworld = Prefab.GetNode<OverworldController>("./OverworldController");
		Overworld.BattleCharacter.Character = character;
		Overworld.BattleCharacter.Character.NodeCharacter = Overworld.Parent;
		Overworld.BattleCharacter.Character.ResourceLocalToScene = true;
		if(character.GetType() == typeof(PartyCharacters))
		{
			if (Data.Party.Contains((PartyCharacters)character))
			{
				Overworld.Parent.Name = $"Party {((PartyCharacterBase)character.Base).PartyId}";				
			}			
		}
		//Overworld.InteractCollider.GetChild<CollisionShape2D>(0).Disabled = true;

		character.SetStats();
		Overworld.SetAnimators();

		Overworld.SetOffsets();
		Overworld.BattleCharacter.SetOffsets();

		GetTree().CurrentScene.AddChild(Prefab);
		return Overworld;
	}
	public void SetCamera(Camera cam)
	{
		OverworldCam = cam;
		AssignCharacterCamera(controller);
	}
	public void SetBattleCamera(Camera cam)
	{
		BattleCam = cam;
		AssignBattleCamera(GetTree().CurrentScene);
	}
	public void SwitchScene(int scene, int Area, Vector2 Position, Color color, int GraphicsLayer, Array<int> CollisionLayer, Array<int> CollisionMask)
	{
		PlayTransition(color);
		TransitionTween.Finished += () =>
		{
			Data.Scene = scene;
			Data.AreaIndex = Area;
			RootCharacters();
			OverworldCam = null;
			BattleCam = null;
			controller?.SetControllable(false);
			PositionToMove = Position;
			GetTree().ChangeSceneToPacked(AreaMaps[Area].maps[scene]);
			SetLayers(GraphicsLayer,CollisionLayer, CollisionMask);

			for(int i = 0; i < Characters.Count; i++)
			{
				for(int j = 0; j < Characters[i].ZIndexList.Count; j++)
				{
					Characters[i].ZIndexList[j] = GraphicsLayer;					
				}
			}
		};
		/*SceneTreeTimer timer = GetTree().CreateTimer(0.5f,true,true,true);
		timer.Timeout+=()=>MoveCharactersToScene(Position);*/
	}
	public void RootCharacters()
	{
		if (IsInstanceValid(OverworldCam))
		{
			OverworldCam?.QueueFree();
			OverworldCam.PositionSmoothingEnabled = true;
		}
		if (IsInstanceValid(BattleCam))
		{
			BattleCam?.QueueFree();
		}
		for (int i = 0; i < Characters.Count; i++)
		{
			Node2D aux = Characters[i].Parent;
			aux.Reparent(GetTree().Root);
			//aux.GetParent().RemoveChild(aux);
			aux.Position = new Vector2(int.MinValue, int.MinValue);

			//GetTree().Root.AddChild(aux);
		}
		for (int i = 0; i < Followers.Count; i++)
		{
			Node2D aux = Followers[i].Parent;
			aux.GetParent().RemoveChild(aux);
			aux.Position = new Vector2(int.MinValue, int.MinValue);
			GetTree().Root.AddChild(aux);
		}
	}
	public void SetDataTileMap(DataTileMap Map)
	{
		for (int i = 0; i < Characters.Count; i++)
		{
			Characters[i].DataMap = Map;
		}
		for (int i = 0; i < Followers.Count; i++)
		{
			Followers[i].DataMap = Map;
		}
	}
	public void MoveCharactersToScene(Scene scene)
	{
		CurrentScene = scene;
		for (int i = 0; i < Characters.Count; i++)
		{
			Node2D aux = Characters[i].Parent;
			aux.GlobalPosition = PositionToMove;
			aux.GetParent().RemoveChild(aux);
			GetTree().CurrentScene.AddChild(aux);
		}
		for (int i = 0; i < Followers.Count; i++)
		{
			Node2D aux = Followers[i].Parent;
			aux.GlobalPosition = PositionToMove;
			aux.GetParent().RemoveChild(aux);
			GetTree().CurrentScene.AddChild(aux);
		}
		Array<Vector2> Aux = controller.PositionList;
		for (int i = 0; i < Aux.Count; i++)
		{
			Aux[i] = controller.GlobalPosition;
		}
	}
	public void Save(int save)
	{
		Settings.CurrentSave = save;
		CurrentSaveExists = true;
		string Path = "user://" + "save" + Settings.CurrentSave.ToString() + ".tres";

		if (controller != null)
		{
			Data.Position = controller.GlobalPosition;
		}
		else
		{
			Data.Position = Vector2.Zero;
		}
		Debug.WriteLine("Scene: " + Data.Scene);
		Saves[Settings.CurrentSave] = Data.DuplicateData();
		if (!Godot.FileAccess.FileExists(Path))
		{
			Godot.FileAccess file = Godot.FileAccess.Open(Path, Godot.FileAccess.ModeFlags.WriteRead);
		}
		ResourceSaver.Save(Saves[Settings.CurrentSave], Path);
		SaveSettings();
		Saves[Settings.CurrentSave].TakeOverPath(Path);
		Debug.WriteLine(Path);
		DisplaySaves();
	}

	public void SaveSettings()
	{
		string SettingsPath = "user://" + "settings" + ".tres";
		ResourceSaver.Save(Settings, SettingsPath);

	}
	public void DisplaySaves()
	{
		for (int i = 0; i < Saves.Count; i++)
		{
			string Path = "user://" + "save" + i.ToString() + ".tres";
			Saves[i] = null;
			if (ResourceLoader.Exists(Path))
			{
				SavesExist = true;
				Saves[i] = (DataManager)ResourceLoader.Load<DataManager>(Path, null, ResourceLoader.CacheMode.Replace).Duplicate(true);
			}
			else
			{
				Saves[i] = null;
			}
		}
		/*string SettingsPath = "user://" + "settings" + ".tres";
        if (ResourceLoader.Exists(SettingsPath))
        {
			Settings = (MainSettings)ResourceLoader.Load<MainSettings>(SettingsPath, null, ResourceLoader.CacheMode.Replace).Duplicate(true);
            if (Saves[Settings.CurrentSave] != null)
			{
				Debug.WriteLine("Exists");
				Data = Saves[Settings.CurrentSave];
            }
        }*/
	}
	public void Load(int Save)
	{
		if (Saves[Save] != null)
		{
			for (int i = 0; i < Characters.Count; i++)
			{
				Characters[i].Parent.Name = "deleting";
			}
			PlayTransition(TransitionColor);
			TransitionTween.Finished += () =>
			{
				EmitSignal("_Load");
				OverworldCam?.GetParent().RemoveChild(OverworldCam);
				CurrentScene?.AddChild(OverworldCam);
				for (int i = 0; i < Characters.Count; i++)
				{
					Characters[i].Parent.QueueFree();
				}
				Data = null;
				Data = Saves[Save].DuplicateData();
				Settings.CurrentSave = Save;
				Characters.Clear();
				Followers.Clear();

				for (int i = 0; i < Data.Party.Count; i++)
				{
					Character Aux = Data.Party[i];
					Aux.ChangeKey(Aux.EventKey);
				}

				Debug.WriteLine("SaveScene :" + Saves[Save].Scene);
				Debug.WriteLine("DataScene :" + Data.Scene);
				Debug.WriteLine("Cam :" + OverworldCam);

				InstantiateCharacters();
				CallDeferred("SwitchScene", Data.Scene, Data.AreaIndex, Data.Position, TransitionColor, Data.GraphicsLayer, Data.CollisionLayer, Data.CollisionMask);				
			};

		}
	}

	public void DeleteSave(int Save)
	{
		Saves.RemoveAt(Save);
	}
	public void Restart()
	{
		Characters.Clear();
		Followers.Clear();
		//Data = null;

		OverworldCam?.QueueFree();
		BattleCam?.QueueFree();
		CurrentScene = null;
		OverworldCam = null;
		BattleCam = null;
		GetTree().ChangeSceneToPacked(StartScene);
	}
	public void SetEXPLevels(PartyCharacterBase CharBase)
    {
        for (int i = 0; i < 100; i++)
        {
            CharBase.ExpForLevel[i] = (int)Mathf.Clamp((int)Mathf.Pow(i / ExpSpeed, ExpDistance)*2,1,Mathf.Inf);
        }
        for (int i = 0; i < 10; i++)
        {
            Debug.WriteLine("Needed EXP: "+CharBase.ExpForLevel[i]);
        }
	}
	public void PlayTransition(Color color)
	{
		TransitionTween = CreateTween();
		TransitionTween.TweenProperty(TransitionAnimator.GetParent<Control>(), "modulate", color, TransitionTime);
		TransitionTween.Finished += TransitionTween.Kill;
		
	}
	
	public void SetLayers(int GraphicsLayer, Array<int> CollisionLayer, Array<int> CollisionMask)
	{
		Data.GraphicsLayer = GraphicsLayer;
		//Data.PhysicsLayer = PhysicsLayer;
		Data.CollisionLayer = CollisionLayer.Duplicate();
		Data.CollisionMask = CollisionMask.Duplicate();

		controller.SetLayers(GraphicsLayer, CollisionLayer, CollisionMask);
		//CHANGE THE MASKS IN THE GAME
		/*	*/
	}

	Tween AudioTween;
	public void PlayAudio(int Stream)
	{
		if (Stream != CurrentBGM && Stream>=0)
		{
			StopAudio();
			CurrentBGM = Stream;
			MainAudio.Stream = BGMs[Stream % BGMs.Count];
			AudioTween.Finished+=()=>
            {
				MainAudio.Play();      
				MainAudio.VolumeDb = 0;          
				/*AudioTween = CreateTween();
				AudioTween.TweenProperty(MainAudio,"volume_db",0,0.2);*/
            };
		}
	}
	public void StopAudio()
    {
        if (AudioTween != null)
        {
			AudioTween.Stop();            
        }
		AudioTween = CreateTween();
		AudioTween.TweenProperty(MainAudio,"volume_db",-80,0.3);
		AudioTween.Finished += MainAudio.Stop;
    }
	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
			Debug.WriteLine("Disposing");

	}
	public static int nfmod(float a,float b)
	{
		return (int)(a - b * Mathf.FloorToInt(a / b));
	}
}


using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public enum BattleState{
	Neutral,
	Win,
	Lose,
	Run
}
public partial class BattleManager : Node
{
	[Export] public BattleScene Scene;
	[Export] BattleStart battleStart;
	[Export] public TypedItemList AuxItems;
	[Export] Array<int> AuxHP = new Array<int>(), EnemyAuxHP = new Array<int>();

	[Export] public BattleState State;
	[Export] Camera2D BattleCamera;
	[Export] public Control TutorialLabels;
	[Export] public LossScreen Loss;
	[Export] public Array<BattleCharacter> Party=new Array<BattleCharacter>(), EnemyParty=new Array<BattleCharacter>(),TurnOrder=new Array<BattleCharacter>();
	public Array<BattleCharacter> UserCharacters=new Array<BattleCharacter>(),TargetCharacters=new Array<BattleCharacter>();
	[Export] Node CharacterButtonParent;
	Array<CharacterButtons> PartyButtons=new Array<CharacterButtons>(), EnemyButtons=new Array<CharacterButtons>();
	[Export] Array<Vector2> partyPos= new Array<Vector2>(), enemyPos=new Array<Vector2>();
	int AliveParty, AliveEnemy;
	public int ActiveMoves, CurrentRound, CurrentTurn=-1, TurnCount = -1;
	public BattleCharacter CurrentCharacter;
	public static BattleManager instance;
	public bool CanStartTurn=true, BattleEnded;
	public Vector2 CenterView,  partyOrigin, enemyOrigin;


	public CharacterButtons CurrentCharacterButton;
	public MoveButtons CurrentMoveButton;
	public ItemButtons CurrentItemButton;

	[Export]double PositionMoveSpeed;
	[Export]public ItemList itemList;
	[Export]public MoveList moveList;
    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        base._EnterTree();
		instance=this; 
    }
    public override void _Ready()
	{
		//GameManager.Instance.SetBattleCamera(BattleCamera);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void StartBattle(BattleScene scene,Array<BattleCharacter> party, Array<BattleCharacter> enemy, Array<Vector2>PartyPos, Array<Vector2> EnemyPos, Vector2 centerViewCam, Vector2 centerViewChar, BattleStart start){
		AuxItems = (TypedItemList)GameManager.Instance.Data.items[0].Duplicate();
		GameManager Game = GameManager.Instance;
		battleStart = start;
		BattleEnded=false;
		//CenterView=centerViewChar;
		CenterView = scene.GetCenterView(PartyPos,EnemyPos);
		/*BattleCamera.Position=centerViewCam;
		BattleCamera.Enabled=true;*/
		Scene=scene;

		for(int i = 0; i<Game.Followers.Count;i++){
			Tween tween = CreateTween();
			tween.SetParallel(true);
			tween.TweenProperty(Game.Followers[i],"modulate:a",0,0.3);
			tween.Finished+=tween.Kill;
		}

		for(int i=0;i<party.Count;i++){
			Party.Add(party[i]);
			partyPos.Add(PartyPos[i]);
			Party[i].OriginPos=partyPos[i];
			Party[i].ThisParty=Party;
			Party[i].EnemyParty=EnemyParty;
			Party[i].PartyButtons=PartyButtons;
			Party[i].EnemyButtons=EnemyButtons;
			AuxHP.Add(Party[i].Character.stats.HP);
			if(((PlayerController)Party[i].Overworld).Leader){
				partyOrigin=Party[i].GlobalPosition;
			}
			TurnOrder.Add(Party[i]);
			AliveParty++;
			CharacterButtons New=new CharacterButtons();
			New.SetBattleCharacter(Party[i]);
			CharacterButtonParent.AddChild(New);
			PartyButtons.Add(New);
			PartyButtons[i].Hide();
			PartyButtons[i].FocusMode=Control.FocusModeEnum.All;
			Party[i].ProcessMode=ProcessModeEnum.Inherit;
		}
		for(int i=0;i<enemy.Count;i++){
			EnemyParty.Add(enemy[i]);
			enemyPos.Add(EnemyPos[i]);
			EnemyParty[i].OriginPos=enemyPos[i];
			EnemyParty[i].ThisParty=EnemyParty;
			EnemyParty[i].EnemyParty=Party;
			EnemyParty[i].PartyButtons=EnemyButtons;
			EnemyParty[i].EnemyButtons=PartyButtons;
			EnemyAuxHP.Add(EnemyParty[i].Character.stats.HP);
			TurnOrder.Add(EnemyParty[i]);

			EnemyParty[i].ProcessMode=ProcessModeEnum.Inherit;

			AliveEnemy++;

			
			CharacterButtons New=new CharacterButtons();
			New.SetBattleCharacter(EnemyParty[i]);
			CharacterButtonParent.AddChild(New);
			EnemyButtons.Add(New);
			EnemyButtons[i].Hide();
			
			EnemyButtons[i].FocusMode=Control.FocusModeEnum.All;
		}

		for(int i=0;i<TurnOrder.Count;i++){
			for (int j=0;j<TurnOrder[i].Character.Equipment.Count;j++){
				TurnOrder[i].Character?.Equipment[j]?.TurnStartEffect(TurnOrder[i]);
			}
		}
		scene.StartBattleEffect();
		ResetPositions();
	}
	public void ResetPositions(){
			if(CurrentTurn==-1){
				//GameManager.Instance.ChangeCam(Scene.GetCenterView(partyPos,enemyPos),true,(float)PositionMoveSpeed);
				GameManager.Instance.ChangeCam(CenterView,true,(float)PositionMoveSpeed);
			}
		Tween tween = CreateTween();
		tween.SetParallel(true);
		

		for(int i=0;i<Party.Count;i++){
			Node2D Parent=Party[i].GetParent<Node2D>();
			ProgressBar HPBar = Party[i].HPBar;
			/*float AuxCenterView = (CenterView.X-partyPos[i].X)/Mathf.Abs(CenterView.X-partyPos[i].X);
			float AuxScale = Parent.Scale.Y/Mathf.Abs(Parent.Scale.Y);*/
			float aux=(CenterView-partyPos[i]).Normalized().X/Parent.Scale.Normalized().Y;

			//float aux=AuxCenterView/AuxScale;
			if(aux<0){
				//Debug.WriteLine("Flipped: "+AuxCenterView+","+AuxScale);
				Parent.Rotation+=Mathf.Pi*Parent.Scale.Y/Mathf.Abs(Parent.Scale.Y);
				Parent.Scale=new Vector2(Parent.Scale.X,Parent.Scale.Y*-1);

			}
			if(Parent.Rotation!=0){
				HPBar.Rotation=Parent.GlobalRotation;
				HPBar.Scale=Parent.GlobalScale;

			}
			else{
				HPBar.Rotation = 0;
				HPBar.Scale = new Vector2(1,1);
				if(Parent.Rotation<0){
					HPBar.Position = new Vector2(Mathf.Abs(HPBar.Position.X),HPBar.Position.Y);
				}
				else{
					HPBar.Position = new Vector2(-Mathf.Abs(HPBar.Position.X),HPBar.Position.Y);
				}
			}
			MoveCharacters(tween,Party[i],partyPos[i],(float)PositionMoveSpeed);
		}

		for(int i=0;i<EnemyParty.Count;i++){
			Node2D Parent=EnemyParty[i].GetParent<Node2D>();
			ProgressBar HPBar = EnemyParty[i].HPBar;
			//float aux=(CenterView-enemyPos[i]).Normalized().X/Parent.Scale.Normalized().Y;
			float AuxCenterView = (CenterView.X-enemyPos[i].X)/Mathf.Abs(CenterView.X-enemyPos[i].X);
			float AuxScale = Parent.Scale.Y/Mathf.Abs(Parent.Scale.Y);
			float aux=AuxCenterView/AuxScale;
			Debug.WriteLine("CenterViewFirst: "+CenterView.X+" Pos: "+enemyPos[i].X);
			Debug.WriteLine("CenterView: "+AuxCenterView + "Scale: "+AuxScale);
			if(aux<0){
				Parent.Rotation+=Mathf.Pi*Parent.Scale.Y/Mathf.Abs(Parent.Scale.Y);
				Parent.Scale=new Vector2(Parent.Scale.X,Parent.Scale.Y*-1);
				}
			if(Parent.Rotation!=0){
				HPBar.Rotation=Parent.GlobalRotation;
				HPBar.Scale=Parent.GlobalScale;
				HPBar.Position = new Vector2(Mathf.Abs(HPBar.Position.X),HPBar.Position.Y);
			}
			else{
				HPBar.Rotation = 0;
				HPBar.Scale = new Vector2(1,1);
				if(Parent.Rotation<0){
					HPBar.Position = new Vector2(Mathf.Abs(HPBar.Position.X),HPBar.Position.Y);
				}
				else{
					HPBar.Position = new Vector2(-Mathf.Abs(HPBar.Position.X),HPBar.Position.Y);
				}
			}
			MoveCharacters(tween,EnemyParty[i],enemyPos[i],(float)PositionMoveSpeed);
		}

		tween.Finished+=EndTurn;
		
		tween.Finished += tween.Kill;
		
	}

	void OrderTurns(){
		Array<BattleCharacter> Aux = TurnOrder.Duplicate();
		IOrderedEnumerable<BattleCharacter> Order = Aux.OrderByDescending(character=>character.Character.TotalStats.Speed);
		TurnOrder.Clear();
		for(int i =0;i<Order.Count();i++){
			TurnOrder.Add(Order.ElementAt(i));
		}
		StartRound();
	}
	void StartRound(){
		CurrentRound++;
		CurrentCharacter=TurnOrder[0];
		for(int i=0;i<TurnOrder.Count;i++){
			for (int j=0;j<TurnOrder[i].Character.Equipment.Count;j++){
				TurnOrder[i].Character?.Equipment[j]?.TurnStartEffect(TurnOrder[i]);
			}
		}
		StartTurn(true);
		Scene.StartRoundEffect();
	}
	public void StartTurn(bool first){
		Scene.StartTurnEffect(first);
		if(CanStartTurn){
			UserCharacters.Add(CurrentCharacter);
			if(Party.Contains(CurrentCharacter)){
				CurrentCharacter.ShowChangeHPBar(CurrentCharacter.Character.stats.HP);
				CurrentCharacter.ShowChangeWPBar(CurrentCharacter.Character.stats.WP,false);
			}
			CurrentCharacter.StartChoosingMove();
		}
	}
	public void EndMove(){
		ActiveMoves--;
		if(ActiveMoves<=0){
			for(int i=0;i<TurnOrder.Count;i++){
				TurnOrder[i].Reset();
			}
			ResetPositions();
		}
	}
	public void CheckDeath(BattleCharacter character){
		TurnOrder.Remove(character);
		if(Party.Contains(character)){
			AliveParty--;
		}
		else if(EnemyParty.Contains(character)){
			AliveEnemy--;
		}
		if(AliveParty==0 || AliveEnemy==0){
			BattleEnded=true;
		}

	}
	public void EndTurn(){
		Scene.DialogueFlag = false;
		TargetCharacters.Clear();
		UserCharacters.Clear();
		ChangeTutorialLabel(0,true, TurnOrder[0].Character);
		if(!BattleEnded){
				CurrentTurn++;
				TurnCount++;
			if(CurrentTurn>=TurnOrder.Count || CurrentTurn ==0){
			Debug.WriteLine("EndRound \n");
				EndRound();
			}
			else{
				Debug.WriteLine("Turn: "+CurrentTurn+"\n");
				CurrentCharacter=TurnOrder[CurrentTurn%TurnOrder.Count];
				if(CurrentCharacter.UsedComboMove){
					CurrentCharacter.UsedComboMove=false;
					EndTurn();
				}
				else{
					StartTurn(true);
				}
			}
		}
		else{
			EndBattle();
		}
	}
	public void EndRound(){
		CurrentTurn=0;
		for(int i=0;i<TurnOrder.Count;i++){
			TurnOrder[i].ReduceMultiplyTimer();
			for (int j=0;j<TurnOrder[i].Character.Equipment.Count;j++){
				TurnOrder[i].Character?.Equipment[j]?.TurnEndEffect(TurnOrder[i]);
			}
		}
		OrderTurns();
	}
	public void EndBattle(){

		if(AliveParty==0){
				State=BattleState.Lose;
			}
		if(AliveEnemy==0){
				State=BattleState.Win;
			}
		Scene.BattleEnd();
	}
	public void Run(){
		State=BattleState.Run;
		Scene.BattleEnd();
	}

	public void ReturnToOverworld(){
		Tween tween=CreateTween();
		Vector2 EndPosition=Vector2.Zero;
		EndPosition=partyOrigin;
		Vector2 EnemyEndPosition=enemyOrigin;
		for(int i=0;i<Party.Count;i++){
			MoveCharacters(tween,Party[i],EndPosition,(float)PositionMoveSpeed);				
			Party[i].AnimatorTree.Set("parameters/conditions/Ended",true);
			Party[i].HideChangeHPBar();
			Party[i].HideChangeWPBar();
			Party[i].Character.stats.WP = 0;
		}
		for(int i=0;i<EnemyParty.Count;i++){
			EnemyParty[i].HideChangeHPBar();
		}		
		if(EnemyParty[0].Character.status!=Character.Status.KO){
			//MoveCharacters(tween,EnemyParty[0],EnemyEndPosition,(float)PositionMoveSpeed);				
			EnemyParty[0].AnimatorTree.Set("parameters/conditions/Ended",true);
		}
		GameManager.Instance.ChangeCam(Vector2.Zero,false,(float)PositionMoveSpeed);
		tween.TweenInterval(0.5);
		//tween.Call("Clear");
		tween.Finished+=Clear;
		tween.Finished+=tween.Kill;
	}
	public void Clear(){
		AuxItems = null;
		for(int i=0;i<CharacterButtonParent.GetChildCount();i++){
			CharacterButtonParent.GetChild(i).QueueFree();
		}
		for(int i=0;i<PartyButtons.Count;i++){
			PartyButtons[i].FocusMode=Control.FocusModeEnum.None;
			PartyButtons[i].Hide();
		}
		for(int i=0;i<EnemyButtons.Count;i++){
			EnemyButtons[i].FocusMode=Control.FocusModeEnum.None;
			EnemyButtons[i].Hide();
		}

		AliveEnemy=0;
		CurrentTurn=-1;
		AliveParty=0;
		/*for(int i=0;i<Party.Count;i++){
			Party[i].OriginPos=Vector2.Zero;
			Party[i].ReturnToOverworld();
		}
		for(int i=0;i<EnemyParty.Count;i++){
			EnemyParty[i].OriginPos=Vector2.Zero;
			EnemyParty[i].ReturnToOverworld();
		}*/
		Scene?.ReturnToOverworld();

		GameManager Game = GameManager.Instance;
		
		for(int i = 0; i<Game.Followers.Count;i++){
			Tween tween = CreateTween();
			tween.SetParallel(true);
			tween.TweenProperty(Game.Followers[i],"modulate:a",1,0.3);
			tween.Finished+=tween.Kill;
		}

		State=BattleState.Neutral;
		Party.Clear();
		EnemyParty.Clear();
		TurnOrder.Clear();
		partyPos.Clear();
		enemyPos.Clear();
		PartyButtons.Clear();
		EnemyButtons.Clear();
		AuxHP.Clear();
		EnemyAuxHP.Clear();
		CenterView=Vector2.Zero;
		//GameManager.Instance.OverworldCam.Enabled=true;
		//BattleCamera.Enabled=false;
		//BattleCamera.Position=Vector2.Zero;
		Scene=null;
	}

	public void MoveCharacters(Tween tween, BattleCharacter character, Vector2 EndPosition, float duration){
		tween.TweenProperty(character.GetParent(),"global_position",EndPosition,duration);
	}
	public void OpenDialogue(){

	}
	public void ChangeTutorialLabel(int Label, bool All, Character character){
		for(int i=0;i<TutorialLabels.GetChildCount();i++){
			TutorialLabels.GetChild<RichTextLabel>(i).Visible=false;
		}
		if(!All){
			RichTextLabel CurrentLabel = TutorialLabels.GetChild<RichTextLabel>(Label);
			CurrentLabel.Visible=true;
			switch (Label){
				case 0:
					CurrentLabel.Text = $"Attack: {character.Key}";
				break;
				case 1:
					CurrentLabel.Text = $"Block: {character.Key}";
				break;
				case 2:
					CurrentLabel.Text = $"Dodge: Arrows + {character.Key}";
				break;
			}
		}
	}
	public void OpenLossScreen(){
		Loss.OpenScreen();
	}
	public void RetryBattle(){
		GameManager.Instance.Data.items[0] = AuxItems;
		State=BattleState.Neutral;
		AliveEnemy=EnemyParty.Count;
		BattleEnded = false;
		CurrentTurn=-1;
		CurrentRound=1;
		AliveParty=Party.Count;
		TurnOrder.Clear();
		for(int i = 0;i<Party.Count;i++){
			Party[i].TurnOnBattle();
			Party[i].Reset();
			Party[i].Character.ChangeHP(AuxHP[i]);
			TurnOrder.Add(Party[i]);
		}
		for(int i = 0;i<EnemyParty.Count;i++){
			EnemyParty[i].TurnOnBattle();
			EnemyParty[i].Reset();
			EnemyParty[i].Character.ChangeHP(EnemyAuxHP[i]);
			TurnOrder.Add(EnemyParty[i]);
		}
		ResetPositions();
		//Clear();
		//battleStart.StartBattle();
	}
}

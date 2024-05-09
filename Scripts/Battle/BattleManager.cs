using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class BattleManager : Node
{
	[Export]public BattleScene Scene;
	[Export]Camera2D BattleCamera;
	public Array<BattleCharacter> Party=new Array<BattleCharacter>(), EnemyParty=new Array<BattleCharacter>(),TurnOrder=new Array<BattleCharacter>();
	public Array<BattleCharacter> UserCharacters=new Array<BattleCharacter>(),TargetCharacters=new Array<BattleCharacter>();
	[Export] Node CharacterButtonParent;
	Array<CharacterButtons> PartyButtons=new Array<CharacterButtons>(), EnemyButtons=new Array<CharacterButtons>();
	[Export] Array<Vector2> partyPos= new Array<Vector2>(), enemyPos=new Array<Vector2>();
	int AliveParty, AliveEnemy, CurrentTurn=-1;
	public int ActiveMoves, CurrentRound;
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
	public override void _Ready()
	{
		instance=this;
		//GameManager.Instance.SetBattleCamera(BattleCamera);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void StartBattle(BattleScene scene,Array<BattleCharacter> party, Array<BattleCharacter> enemy, Array<Vector2>PartyPos, Array<Vector2> EnemyPos, Vector2 centerViewCam, Vector2 centerViewChar){
		BattleEnded=false;
		CenterView=centerViewChar;
		/*BattleCamera.Position=centerViewCam;
		BattleCamera.Enabled=true;*/
		Scene=scene;
		for(int i=0;i<party.Count;i++){
			Party.Add(party[i]);
			partyPos.Add(PartyPos[i]);
			Party[i].OriginPos=partyPos[i];
			Party[i].ThisParty=Party;
			Party[i].EnemyParty=EnemyParty;
			Party[i].PartyButtons=PartyButtons;
			Party[i].EnemyButtons=EnemyButtons;
			if(Party[i].Overworld.Leader){
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
				TurnOrder[i].Character.Equipment[j].Base.TurnStartEffect(TurnOrder[i]);
			}
		}
		scene.StartBattleEffect();
		ResetPositions();
	}
	public void ResetPositions(){
			if(CurrentTurn==-1){
				GameManager.Instance.ChangeCam(Scene.GetCenterView(partyPos,enemyPos),true,(float)PositionMoveSpeed);
			}
		Tween tween = CreateTween();
		tween.SetParallel(true);
		

		for(int i=0;i<Party.Count;i++){
			Node2D Parent=Party[i].GetParent<Node2D>();
			float aux=(CenterView-partyPos[i]).Normalized().X/Parent.Scale.Normalized().Y;
			if(aux<0){
				Parent.Rotation+=Mathf.Pi*Parent.Scale.Y/Mathf.Abs(Parent.Scale.Y);
				Parent.Scale=new Vector2(Parent.Scale.X,Parent.Scale.Y*-1);
			}
			MoveCharacters(tween,Party[i],partyPos[i],(float)PositionMoveSpeed);
			//tween.TweenProperty(Party[i].GetParent(),"position",partyPos[i],PositionMoveSpeed);
		}
		for(int i=0;i<EnemyParty.Count;i++){
			Node2D Parent=EnemyParty[i].GetParent<Node2D>();
			float aux=(CenterView-enemyPos[i]).Normalized().X/Parent.Scale.Normalized().Y;
			if(aux<0){
				Parent.Rotation+=Mathf.Pi*Parent.Scale.Y/Mathf.Abs(Parent.Scale.Y);
				Parent.Scale=new Vector2(Parent.Scale.X,Parent.Scale.Y*-1);
				}
			//tween.TweenProperty(EnemyParty[i].GetParent(),"position",enemyPos[i],PositionMoveSpeed);
			MoveCharacters(tween,EnemyParty[i],enemyPos[i],(float)PositionMoveSpeed);
		}

		tween.Finished+=EndTurn;
		
		tween.Finished += tween.Kill;
		
	}

	void OrderTurns(){
		TurnOrder.OrderBy(character=>character.Character.stats.Speed);
		StartRound();
	}
	void StartRound(){
		CurrentRound++;
		CurrentCharacter=TurnOrder[0];
		for(int i=0;i<TurnOrder.Count;i++){
			for (int j=0;j<TurnOrder[i].Character.Equipment.Count;j++){
				TurnOrder[i].Character.Equipment[j].Base.TurnStartEffect(TurnOrder[i]);
			}
		}
		StartTurn(true);
		Scene.StartRoundEffect();
	}
	public void StartTurn(bool first){
		Scene.StartTurnEffect(first);
		if(CanStartTurn){
			UserCharacters.Add(CurrentCharacter);
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
		Debug.WriteLine("PartyAlive: "+AliveParty);
		Debug.WriteLine("EnemyAlive: "+AliveEnemy);

		TargetCharacters.Clear();
		UserCharacters.Clear();
		if(!BattleEnded){
				CurrentTurn++;
			if(CurrentTurn==TurnOrder.Count){
			Debug.WriteLine("EndRound \n");
				EndRound();
			}
			else{
				Debug.WriteLine("Turn: "+CurrentTurn+"\n");
				CurrentCharacter=TurnOrder[CurrentTurn];
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
				TurnOrder[i].Character.Equipment[j].Base.TurnEndEffect(TurnOrder[i]);
			}
		}
		OrderTurns();
	}
	public void EndBattle(){

		if(AliveParty==0){
				Scene.LoseEffect();
			}
		if(AliveEnemy==0){
				Scene.WinEffect();
			}
	}
	public void Run(){
		Scene.RunEffect();
	}

	public void ReturnToOverworld(){
		Tween tween=CreateTween();
		Vector2 EndPosition=Vector2.Zero;
		EndPosition=partyOrigin;
		for(int i=0;i<Party.Count;i++){
			MoveCharacters(tween,Party[i],EndPosition,(float)PositionMoveSpeed);				
			Party[i].AnimatorTree.Set("parameters/conditions/Ended",true);
		}
		GameManager.Instance.ChangeCam(Vector2.Zero,false,(float)PositionMoveSpeed);
		tween.TweenInterval(0.5);
		//tween.Call("Clear");
		tween.Finished+=Clear;
		tween.Finished+=tween.Kill;
	}
	public void Clear(){

		for(int i=0;i<EnemyParty.Count;i++){
			EnemyParty[i].OriginPos=Vector2.Zero;
			EnemyParty[i].TurnOffBattle();
		}
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
		for(int i=0;i<Party.Count;i++){
			Party[i].OriginPos=Vector2.Zero;
			Party[i].TurnOffBattle();
		}

		AliveEnemy=0;
		CurrentTurn=-1;
		AliveParty=0;
		Party.Clear();
		EnemyParty.Clear();
		TurnOrder.Clear();
		partyPos.Clear();
		enemyPos.Clear();
		PartyButtons.Clear();
		EnemyButtons.Clear();
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
}

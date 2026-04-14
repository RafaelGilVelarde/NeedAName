using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

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
	[Export] public TypedItemList AuxItems = new TypedItemList();
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
	public int ActiveMoves, CurrentRound, CurrentTurn=-1, TurnCount = 0;
	public BattleCharacter CurrentCharacter;
	public static BattleManager instance;
	public bool CanStartTurn=true, BattleEnded;
	public Vector2 CenterView,  partyOrigin, enemyOrigin;


	public CharacterButtons CurrentCharacterButton;
	public MoveButtons CurrentMoveButton;
	public ItemButtons CurrentItemButton;

	[Export]double PositionMoveSpeed;
	[Export] public TextureRect MenuUI,TutorialUI;
	[Export]public ItemList itemList;
	[Export]public MoveList moveList;
    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        base._EnterTree();
		instance=this; 
		AuxItems.items = new Array<Items>();
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
		GameManager Game = GameManager.Instance;
		for(int i = 0; i < Game.Data.items[0].items.Count; i++)
		{
			AuxItems.AddItem((Items)Game.Data.items[0].items[i].Duplicate());
		}
		
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
			Party[i].PosIndex = i;
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
			EnemyParty[i].PosIndex = i;
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
			Debug.WriteLine("Turn "+i+": "+TurnOrder[i].Character.Base.Name);
		}
		scene.SetupBattleEffect();
		ResetPositions();
	}

	void SetCharacterButtonNav()
	{
		IOrderedEnumerable<Vector2> PartyHOrder = partyPos.OrderBy(x => x.X);
		IOrderedEnumerable<Vector2> PartyVOrder = partyPos.OrderByDescending(y => y.Y);

		IOrderedEnumerable<Vector2> EnemyHOrder = enemyPos.OrderBy(x => x.X);
		IOrderedEnumerable<Vector2> EnemyVOrder = enemyPos.OrderByDescending(y => y.Y);

		Array<int> PartyH = new Array<int>(), PartyV = new Array<int>(), EnemyH = new Array<int>(), EnemyV = new Array<int>();

		for (int i = 0; i < PartyHOrder.Count(); i++)
		{
			PartyH.Add(partyPos.IndexOf(PartyHOrder.ElementAt(i)));
			PartyV.Add(partyPos.IndexOf(PartyVOrder.ElementAt(i)));
		}

		for (int i = 0; i < EnemyHOrder.Count(); i++)
		{
			EnemyH.Add(enemyPos.IndexOf(EnemyHOrder.ElementAt(i)));
			EnemyV.Add(enemyPos.IndexOf(EnemyVOrder.ElementAt(i)));
		}

		for (int i = 0; i < PartyButtons.Count; i++)
		{
			int AuxH = PartyH.IndexOf(i);
			int AuxV = PartyV.IndexOf(i);

			PartyButtons[i].FocusNeighborLeft = PartyButtons[PartyH[GameManager.nfmod(AuxH - 1,PartyButtons.Count)]].GetPath();
			PartyButtons[i].FocusNeighborRight = PartyButtons[PartyH[GameManager.nfmod(AuxH + 1, PartyButtons.Count)]].GetPath();
			PartyButtons[i].FocusNeighborTop = PartyButtons[PartyV[GameManager.nfmod(AuxV + 1, PartyButtons.Count)]].GetPath();
			PartyButtons[i].FocusNeighborBottom = PartyButtons[PartyV[GameManager.nfmod(AuxV - 1, PartyButtons.Count)]].GetPath();
		}

		for (int i = 0; i < EnemyButtons.Count; i++)
		{
			int AuxH = EnemyH.IndexOf(i);
			int AuxV = EnemyV.IndexOf(i);
			EnemyButtons[i].FocusNeighborLeft = EnemyButtons[EnemyH[GameManager.nfmod(AuxH - 1, EnemyButtons.Count)]].GetPath();
			EnemyButtons[i].FocusNeighborRight = EnemyButtons[EnemyH[GameManager.nfmod(AuxH + 1, EnemyButtons.Count)]].GetPath();
			EnemyButtons[i].FocusNeighborTop = EnemyButtons[EnemyV[GameManager.nfmod(AuxV + 1, EnemyButtons.Count)]].GetPath();
			EnemyButtons[i].FocusNeighborBottom = EnemyButtons[EnemyV[GameManager.nfmod(AuxV - 1, EnemyButtons.Count)]].GetPath();
		}
	}

	public void ResetPositions()
	{
		if (CurrentTurn == -1)
		{
			//GameManager.Instance.ChangeCam(Scene.GetCenterView(partyPos,enemyPos),true,(float)PositionMoveSpeed);
			GameManager.Instance.ChangeCam(CenterView, true, (float)PositionMoveSpeed);
		}
		Tween tween = CreateTween();
		tween.SetParallel(true);


		for (int i = 0; i < Party.Count; i++)
		{
			Node2D Parent = Party[i].GetParent<Node2D>();
			ProgressBar HPBar = Party[i].HPBar;
			ProgressBar WPBar = Party[i].WPBar;
			HBoxContainer StatMods = Party[i].StatMods;
			/*float AuxCenterView = (CenterView.X-partyPos[i].X)/Mathf.Abs(CenterView.X-partyPos[i].X);
			float AuxScale = Parent.Scale.Y/Mathf.Abs(Parent.Scale.Y);*/
			float aux = (CenterView - partyPos[i]).Normalized().X / Parent.Scale.Normalized().Y;

			//float aux=AuxCenterView/AuxScale;
			if (aux < 0)
			{
				Parent.Rotation += Mathf.Pi * Parent.Scale.Y / Mathf.Abs(Parent.Scale.Y);
				Parent.Scale = new Vector2(Parent.Scale.X, Parent.Scale.Y * -1);
			}
			SetScale(HPBar, aux, Parent);
			SetScale(WPBar, aux, Parent);
			SetScale(StatMods, aux, Parent);

			MoveCharacters(tween, Party[i], partyPos[i], (float)PositionMoveSpeed);
		}

		for (int i = 0; i < EnemyParty.Count; i++)
		{
			Node2D Parent = EnemyParty[i].GetParent<Node2D>();
			ProgressBar HPBar = EnemyParty[i].HPBar;
			HBoxContainer StatMods = EnemyParty[i].StatMods;
			//float aux=(CenterView-enemyPos[i]).Normalized().X/Parent.Scale.Normalized().Y;
			float AuxCenterView = (CenterView.X - enemyPos[i].X) / Mathf.Abs(CenterView.X - enemyPos[i].X);
			float AuxScale = Parent.Scale.Y / Mathf.Abs(Parent.Scale.Y);
			float aux = AuxCenterView / AuxScale;


			if (aux < 0)
			{
				Parent.Rotation += Mathf.Pi * Parent.Scale.Y / Mathf.Abs(Parent.Scale.Y);
				Parent.Scale = new Vector2(Parent.Scale.X, Parent.Scale.Y * -1);
			}
			SetScale(HPBar, aux, Parent);
			SetScale(StatMods, aux, Parent);
			MoveCharacters(tween, EnemyParty[i], enemyPos[i], (float)PositionMoveSpeed);
		}

		tween.Finished += EndTurn;

		tween.Finished += tween.Kill;

		void SetScale(Control Bar, float aux, Node2D Parent)
		{
			Node2D control = Bar.GetParent<Node2D>();
			if (Parent.Rotation != 0)
			{
				control.Rotation = Parent.GlobalRotation;
				control.Scale = Parent.GlobalScale;
			}
			else
			{
				control.Rotation = 0;
				control.Scale = new Vector2(1, 1);
				if (Parent.Rotation < 0)
				{
					control.Position = new Vector2(Mathf.Abs(control.Position.X), control.Position.Y);
				}
				else
				{
					control.Position = new Vector2(-Mathf.Abs(control.Position.X), control.Position.Y);
				}

			}
		}
	}

	void OrderTurns(){
		Array<BattleCharacter> Aux = TurnOrder.Duplicate();
		IOrderedEnumerable<BattleCharacter> Order = Aux.OrderByDescending(character=>character.Character.TotalStats.Speed);
		TurnOrder.Clear();
		for(int i =0;i<Order.Count();i++){
			TurnOrder.Add(Order.ElementAt(i));
		}
		SetCharacterButtonNav();
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
		StartTurn();
		Scene.StartRoundEffect();
	}
	public void StartTurn(){
		Scene.StartTurnEffect();
		if(UserCharacters.Count == 0){
			UserCharacters.Add(CurrentCharacter);
			if(Party.Contains(CurrentCharacter)){
				CurrentCharacter.ShowChangeHPBar(CurrentCharacter.Character.stats.HP);
				CurrentCharacter.ShowChangeWPBar(CurrentCharacter.Character.stats.WP,false);
				
				CurrentCharacter.ShowStatsMods();
			}
		}
		if(CanStartTurn){
			CurrentCharacter.ReduceStatMultiplier();
			CurrentCharacter.StartChoosingMove();
		}
	}
	public void EndMove(){
		ActiveMoves--;
		if(ActiveMoves<=0){
			ActiveMoves = 0;
			for (int i = 0; i < TurnOrder.Count; i++)
			{
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
				EndRound();
			}
			else{
				CurrentCharacter=TurnOrder[CurrentTurn%TurnOrder.Count];
				if(CurrentCharacter.UsedComboMove){
					CurrentCharacter.UsedComboMove=false;
					EndTurn();
				}
				else{
					StartTurn();
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
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int Random = RNG.RandiRange(0,99);
		CurrentCharacter.selectActions.ProcessMode=ProcessModeEnum.Disabled;
		if (Scene.EscapeChance > Random)
		{
			State = BattleState.Run;
			Scene.BattleEnd();
		}
		else
		{
			EndMove();
		}
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
			Party[i].HideStatsMods();
			Party[i].HideChangeWPBar();
			Party[i].Character.stats.WP = 0;
		}
		for(int i=0;i<EnemyParty.Count;i++){
			EnemyParty[i].HideChangeHPBar();
			EnemyParty[i].HideStatsMods();
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
		AuxItems.items.Clear();
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
		TurnCount = 0;
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
		TutorialUI.Hide();
		if(!All){
			RichTextLabel CurrentLabel = TutorialLabels.GetChild<RichTextLabel>(Label);
			CurrentLabel.Visible=true;
			TutorialUI.Show();
			switch (Label){
				case 0:
					CurrentLabel.Text = Tr("Attack: ")+character.Key;
				break;
				case 1:
					CurrentLabel.Text = Tr("Block: ")+character.Key;
				break;
				case 2:
					CurrentLabel.Text = Tr("Dodge: Arrows + ")+character.Key;
				break;
			}
		}
	}
	public void OpenLossScreen(){
		Loss.OpenScreen();
	}
	public void RetryBattle(){
		GameManager.Instance.Data.items[0].items.Clear();
		for(int i = 0; i < AuxItems.items.Count; i++)
		{
			GameManager.Instance.Data.items[0].AddItem((Items)AuxItems.items[i].Duplicate());
		}
		State=BattleState.Neutral;
		AliveEnemy=EnemyParty.Count;
		BattleEnded = false;
		CurrentTurn=-1;
		CurrentRound=1;
		AliveParty=Party.Count;
		TurnOrder.Clear();
		for(int i = 0;i<Party.Count;i++){
			Party[i].TurnOnBattle();
			//Party[i].Reset();
			Party[i].Character.stats.HP = AuxHP[i];
			Party[i].Character.stats.WP = 0;
			TurnOrder.Add(Party[i]);
		}
		for(int i = 0;i<EnemyParty.Count;i++){
			EnemyParty[i].TurnOnBattle();
			//EnemyParty[i].Reset();
			EnemyParty[i].Character.ResetCharacter();
			TurnOrder.Add(EnemyParty[i]);
		}
		ResetPositions();
		Scene.StartBattleEffect();
		//Clear();
		//battleStart.StartBattle();
	}
}

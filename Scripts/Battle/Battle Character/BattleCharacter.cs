using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;

public partial class BattleCharacter : CharacterBody2D
{

	[Export] public AnimationTree AnimatorTree;
	[Export] public AnimationPlayer AnimatorPlayer;

	[Export]public Character Character;
	[Export]public Hitbox Hitbox;
	[Export]public Area2D Hurtbox, Blockbox, WPbox;
	[Export] public bool Controllable;
	[Export] public OverworldController Overworld;
	[Export]public Godot.Vector2 OriginPos;

	[Export]public Array<float> MultiplierAtk,MultiplierDef,MultiplierSpAtk,MultiplierSpDef,MultiplierSpeed;
	[Export]public int Combo=1, HoldingMoveTimer=0;
	[Export] public bool HoldingMove, UsedComboMove, BlockedEnemy, Looping;
	[Export]public Array<int> TimerAtk,TimerDef,TimerSpAtk,TimerSpDef,TimerSpeed;
	[Export]public Array<CharacterButtons> PartyButtons, EnemyButtons;
	[Export]SelectActions selectActions;
	[Export]public Godot.Vector2 DodgeDir;
	[Export]public Moves MoveUsed;
	[Export]public Sprite2D MainSprite;
	[Export]public Array<BattleCharacter> ThisParty,EnemyParty;
	[Export] Key SelectKey;
	[Export] StateParticleEffects StateParticles;
	[Export] CpuParticles2D HitParticles;
	[Export] public Node2D ShootNode;
	[Export] RichTextLabel HPText, NameText, WPText;
	[Export] public Consumables CurrentItem;
	[Export] public ProgressBar HPBar, EXPBar, WPBar, WPAuxBar;


	[Signal]
	public delegate void _ReturnToIdleEventHandler(BattleCharacter character);
	[Signal]
	public delegate void _DoActionEventHandler();
	[Signal]
	public delegate void _ShootEventHandler(BattleCharacter character);

	Tween DodgeTween;


	

	public enum BattleState{
		Attacking,
		Defending,
		Dodging,
		Idle,
	}
		public enum ActionState{
		isAttacking,
		isDefending,
		isDodging,
		isHit,
		isBlocked,
		isIdle,
		isDead
	}
	[Export]public BattleState battleState;
	[Export]public ActionState actionState;

    public override void _Ready()
	{
		HPText = HPBar.GetChild<RichTextLabel>(0);
		NameText = HPBar.GetChild<RichTextLabel>(1);
		WPText = WPBar?.GetChild<RichTextLabel>(1);
	}
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

		if(Character.isControlledByPlayer&&Controllable){
			PartyCharacterBase aux=(PartyCharacterBase)Character.Base;

			Godot.Vector2 Axis;
			Axis = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
			Axis.X*=GlobalScale.Y;


			if(battleState==BattleState.Dodging && actionState!=ActionState.isDodging){
				//Godot.Vector2 AxisAux=new Godot.Vector2(Axis.X/Scale.Y,Axis.Y);
				DodgeDir=Axis.Normalized();
				if(GlobalScale.Y>0){
					DodgeDir.X=Mathf.Clamp(DodgeDir.X,-1,0);
				}
				else{
					DodgeDir.X=Mathf.Clamp(DodgeDir.X,0,1);
				}
				AnimatorTree.Set("parameters/ActionState/2/blend_position",DodgeDir);
			}
			if(Input.IsActionJustPressed("SelectedKey"+aux.PartyId)&&actionState==ActionState.isIdle){
				switch(battleState){
					case BattleState.Attacking:
						//changeAction(ActionState.isAttacking);
						EmitSignal("_DoAction");
					break;
					case BattleState.Defending:
						changeAction(ActionState.isDefending);
					break;
					case BattleState.Dodging:
						if(actionState!=ActionState.isDodging && DodgeDir!=Godot.Vector2.Zero &&(DodgeDir.X*DodgeDir.Y==0)){
							changeAction(ActionState.isDodging);

							DodgeTween=CreateTween();
							DodgeTween.TweenProperty(this,"position",DodgeDir*20,0.2f);
							DodgeTween.Finished+=DodgeTween.Kill;
						}
					break;
				}
			}
			/*if(actionState==ActionState.isDodging&&Axis!=Godot.Vector2.Zero){
				Position = new Godot.Vector2(Position.X,Position.Y).MoveToward(OriginPos+DodgeDir*3, 250);
			}
			if(battleState==BattleState.Dodging && (actionState==ActionState.isHit||Axis==Godot.Vector2.Zero)){
				Position = new Godot.Vector2(Position.X,Position.Y).MoveToward(OriginPos, 250);
				if(actionState!=ActionState.isHit){
					actionState=ActionState.isIdle;
				}
			}*/
		}
    }
    public override void _Process(double delta)
	{




	}
	public void changeAction(ActionState state){

		AnimatorPlayer.Play("RESET");
		actionState=state;
		AnimatorTree.Set("parameters/ActionState/blend_position",(int)actionState);
	}
	public void changeState(BattleState state){
		if(Character.isControlledByPlayer && (int)state<3){
			BattleManager.instance.ChangeTutorialLabel((int)state,false, Character);
		}
		AnimatorPlayer.Play("RESET");
		StateParticles.EmitParticles(state);
		battleState=state;
	}
	public void changeCombo(int combo){
		Combo = combo;
		AnimatorTree.Set("parameters/ActionState/0/0/blend_position",Combo);
	}
	public void UseMove(Moves move){
		for (int i=0;i<Character.Equipment.Count;i++){
			Character?.Equipment[i]?.ActivateMoveEffect(this,move.Base);
		}
		MoveUsed=move;
		move.Base.Effect(BattleManager.instance.UserCharacters,BattleManager.instance.TargetCharacters);
		Controllable=true;
		for(int i=0;i<BattleManager.instance.TargetCharacters.Count;i++){
			BattleManager.instance.TargetCharacters[i].Hurtbox.GetChild<CollisionShape2D>(0).Disabled=false;
			if(BattleManager.instance.TargetCharacters[i].Character.isControlledByPlayer){
				BattleManager.instance.TargetCharacters[i].WPbox.GetChild<CollisionShape2D>(0).Disabled=false;
			}
		}
		for(int i=0;i<BattleManager.instance.TurnOrder.Count;i++){
			BattleManager.instance.TurnOrder[i].HideChangeHPBar();
		}
		if(Character.isControlledByPlayer){
			Character.ChangeWP(-move.Base.Cost);
		}
		selectActions.ProcessMode=ProcessModeEnum.Disabled;
	}
	
	public void UseItem(Items item){
		Array<BattleCharacter> Aux = BattleManager.instance.TargetCharacters;
		Array<Character> characters = new Array<Character>();
		for(int i =0;i<Aux.Count;i++){
			characters.Add(Aux[i].Character);
		}
		item.Base.Effect(characters);
	}
	public void Shoot(){
		EmitSignal("_Shoot",this);
	}
	public virtual void StartChoosingMove(){
		if(!HoldingMove){
			selectActions.character=this;
			BattleManager.instance.moveList.character=this;
			BattleManager.instance.itemList.character=this;

			selectActions.ProcessMode=ProcessModeEnum.Inherit;
			selectActions.Start();
		}
		else if(HoldingMoveTimer==0){
			UseMove(MoveUsed);
		}
		else{
			Reset();
			BattleManager.instance.EndTurn();
		}
	}

	/*void SetIdle(bool idle){
		AnimatorTree.Set("parameters/conditions/Idle",idle);
		AnimatorTree.Set("parameters/conditions/notIdle",!idle);
	}*/
	public void AddAttack(){
		_DoAction+=ChangeToAttack;
	}
	public void RemoveAttack(){
		_DoAction-=ChangeToAttack;
	}
	void ChangeToAttack(){
		changeAction(ActionState.isAttacking);
	}
	void ReturnToIdle(){
		if(!Looping){
			ReturnLocalPos();
			//AnimatorTree.Active=false;
			//AnimatorTree.Active=true;
			changeAction(ActionState.isIdle);
			BlockedEnemy=false;
			Blockbox.GetChild<CollisionShape2D>(0).Disabled=true;
			//SetIdle(true);
			EmitSignal("_ReturnToIdle",this);
		}
	}
	public void Reset(){
		CurrentItem = null;
		changeState(BattleState.Idle);
		changeAction(ActionState.isIdle);
		Looping=false;
		ReturnToIdle();
		HideChangeHPBar();
		changeCombo(1);
		Controllable=false;
		BlockedEnemy=false;
		Hitbox.GetChild<CollisionShape2D>(0).Disabled=true;
		Hurtbox.GetChild<CollisionShape2D>(0).Disabled=true;
		Blockbox.GetChild<CollisionShape2D>(0).Disabled=true;
		if(WPbox!=null){
			WPbox.GetChild<CollisionShape2D>(0).Disabled=true;			
		}
		if(!HoldingMove){
			MoveUsed=null;
		}else{
			HoldingMoveTimer--;
		}
	}
	protected void Die(){
		Looping=true;
		Hurtbox.GetChild<CollisionShape2D>(0).SetDeferred("disabled",true);
		changeAction(ActionState.isDead);
		//HitParticles.Emitting=true;
		BattleManager.instance.CheckDeath(this);
	}


	public virtual void StartChoosingUsers(Moves move){

	}

	public virtual void StartChoosingTarget(Moves move){

	}


	protected void GetHit(){
		changeAction(ActionState.isHit);
		if(battleState==BattleState.Dodging){
			if(DodgeTween!=null){
				DodgeTween.Kill();
			}
			ReturnLocalPos();
		}	
	}
	void ReturnLocalPos(){
			Tween tween=CreateTween();
			tween.TweenProperty(this,"position",Godot.Vector2.Zero,0.2f);
			tween.Finished+=tween.Kill;
	}

	public void ShowChangeHPBar(int HP){
		Tween tween = CreateTween();
		HPText.Text = $"[center]{Character.stats.HP}/{Character.TotalStats.MaxHP}[/center]";
		NameText.Text = $"[center]{Character.Base.Name}[/center]";
		
		tween.TweenProperty(HPBar,"modulate:a",1,0.1f);
		tween.TweenProperty(HPBar,"value",Character.stats.HP,0.3f).SetEase(Tween.EaseType.InOut);
		
		if(Character.stats.HP<=0){
			tween.Finished+=HideChangeHPBar;
		}
		tween.Finished+=tween.Kill;
	}
	public void HideChangeHPBar(){
		Tween tween = CreateTween();
		tween.TweenProperty(HPBar,"modulate:a",0,0.2f);
	}
	public void ShowChangeWPBar(int WP, bool Hide){
		Tween tween = CreateTween();
		WPText.Text = $"[center]{Character.stats.WP}/100[/center]";
		tween.TweenProperty(WPBar,"modulate:a",1,0.1f);
		tween.TweenProperty(WPBar,"value",WP,0.3f).SetEase(Tween.EaseType.InOut);
		if(Hide){
			tween.Finished+=HideChangeWPBar;
		}
		tween.Finished+=tween.Kill;
	}
	public void HideChangeWPBar(){
		Tween tween = CreateTween();
		tween.TweenProperty(WPBar,"modulate:a",0,0.1f);
		tween.Finished+=()=>{WPAuxBar.Value = 0;};
	}
	public void ShowChangeWPAuxBar(int WP){
		Tween tween = CreateTween();
		tween.TweenProperty(WPAuxBar,"value",Character.stats.WP-WP,0.3f).SetEase(Tween.EaseType.InOut);
		
		tween.Finished+=tween.Kill;
	}

	public void ShowEXPBar(int ExpStart, int ExpEnd, int NextLvl){
		PartyCharacters Aux = (PartyCharacters) Character;
		EXPBar.MaxValue = NextLvl;
		EXPBar.Value = ExpStart-Aux.PastLevelExp;
		Tween tween = CreateTween();
		tween.TweenProperty(EXPBar,"modulate:a",1,0.1f);
		tween.TweenProperty(EXPBar,"value",ExpEnd-Aux.PastLevelExp,0.3f).SetEase(Tween.EaseType.InOut);
		if(ExpEnd>Aux.NextLevelExp){
			tween.Finished+=()=>{
				ShowEXPBar(NextLvl,ExpEnd, (int)Mathf.Pow(Aux.stats.Lv/((PartyCharacterBase)Aux.Base).ExpSpeed,((PartyCharacterBase)Aux.Base).ExpDistance));
			};
		}
	}
	public void HideEXPBar(){
		Tween tween = CreateTween();
		tween.TweenProperty(EXPBar,"modulate:a",0,0.2f);
	}
	public void AddAtkMultiplier(float Multiplier, int Timer){
		MultiplierAtk.Add(Multiplier);
		TimerAtk.Add(Timer);
	}
	public void AddDefMultiplier(float Multiplier, int Timer){
		MultiplierDef.Add(Multiplier);
		TimerDef.Add(Timer);
	}
	public void AddSpAtkMultiplier(float Multiplier, int Timer){
		MultiplierSpAtk.Add(Multiplier);
		TimerSpAtk.Add(Timer);
	}
	public void AddSpDefMultiplier(float Multiplier, int Timer){
		MultiplierSpDef.Add(Multiplier);
		TimerSpDef.Add(Timer);
	}
	public void AddSpeedMultiplier(float Multiplier, int Timer){
		MultiplierSpeed.Add(Multiplier);
		TimerSpeed.Add(Timer);
	}



	public float MultiplyAtk(){
		float aux=1;
		for(int i=0;i<MultiplierAtk.Count;i++){
			aux*=MultiplierAtk[i];
		}
		return aux;
	}
	public float MultiplyDef(){
		float aux=1;
		for(int i=0;i<MultiplierDef.Count;i++){
			aux*=MultiplierDef[i];
		}
		return aux;
	}
		public float MultiplySpAtk(){
		float aux=1;
		for(int i=0;i<MultiplierSpAtk.Count;i++){
			aux*=MultiplierSpAtk[i];
		}
		return aux;
	}
		public float MultiplySpDef(){
		float aux=1;
		for(int i=0;i<MultiplierSpDef.Count;i++){
			aux*=MultiplierSpDef[i];
		}
		return aux;
	}
		public float MultiplySpeed(){
		float aux=1;
		for(int i=0;i<MultiplierSpeed.Count;i++){
			aux*=MultiplierSpeed[i];
		}
		return aux;
	}
	public void ReduceMultiplyTimer(){
		Array<int>Atk=new Array<int>();
		Array<int>Def=new Array<int>();
		Array<int>SpAtk=new Array<int>();
		Array<int>SpDef=new Array<int>();
		Array<int>Speed=new Array<int>();
		for(int i=1;i<MultiplierAtk.Count;i++){
			TimerAtk[i]--;
			if(TimerAtk[i]<=0){
				Atk.Add(i);
			}
		}
		for(int i=1;i<MultiplierDef.Count;i++){
			TimerDef[i]--;
			if(TimerDef[i]<=0){
				Def.Add(i);
			}
		}
		for(int i=1;i<MultiplierSpAtk.Count;i++){
			TimerSpAtk[i]--;
			if(TimerSpAtk[i]<=0){
				SpAtk.Add(i);
			}
		}
		for(int i=1;i<MultiplierSpDef.Count;i++){
			TimerSpDef[i]--;
			if(TimerSpDef[i]<=0){
				SpDef.Add(i);
			}
		}
		for(int i=1;i<MultiplierSpeed.Count;i++){
			TimerSpeed[i]--;
			if(TimerSpeed[i]<=0){
				Speed.Add(i);
			}
		}

		for(int i=1;i<Atk.Count;i++){
			TimerAtk.RemoveAt(Atk[i]);
			MultiplierAtk.RemoveAt(Atk[i]);
		}
		for(int i=1;i<Def.Count;i++){
			TimerSpAtk.RemoveAt(Def[i]);
			MultiplierAtk.RemoveAt(Def[i]);
		}
		for(int i=1;i<SpAtk.Count;i++){
			TimerSpAtk.RemoveAt(SpAtk[i]);
			MultiplierSpAtk.RemoveAt(SpAtk[i]);
		}
		for(int i=1;i<SpDef.Count;i++){
			TimerSpAtk.RemoveAt(SpDef[i]);
			MultiplierSpAtk.RemoveAt(SpDef[i]);
		}
		for(int i=1;i<Speed.Count;i++){
			TimerSpeed.RemoveAt(Atk[i]);
			MultiplierSpeed.RemoveAt(Atk[i]);
		}
	}
	public void ClearTimers(){
		HoldingMoveTimer = 0;
		for(int i=1;i<MultiplierAtk.Count;i++){
			TimerAtk.RemoveAt(i);
			MultiplierAtk.RemoveAt(i);
		}
		for(int i=1;i<MultiplierDef.Count;i++){
			TimerSpAtk.RemoveAt(i);
			MultiplierAtk.RemoveAt(i);
		}
		for(int i=1;i<MultiplierSpAtk.Count;i++){
			TimerSpAtk.RemoveAt(i);
			MultiplierSpAtk.RemoveAt(i);
		}
		for(int i=1;i<MultiplierSpDef.Count;i++){
			TimerSpAtk.RemoveAt(i);
			MultiplierSpAtk.RemoveAt(i);
		}
		for(int i=1;i<MultiplierSpeed.Count;i++){
			TimerSpeed.RemoveAt(i);
			MultiplierSpeed.RemoveAt(i);
		}
	}

	public void Seek(string path, float offset){
		if(Looping){
			AnimatorTree.Set(path, offset);
		}
	}

	public void TurnOnBattle(){
		ProcessMode=ProcessModeEnum.Inherit;
		HPText.Text = $"[center]{Character.stats.HP}/{Character.TotalStats.MaxHP}[/center]";
		Show();
		AnimatorTree.Active=false;
		AnimatorTree.Active=true;
		HPBar.MaxValue = Character.TotalStats.MaxHP;
		Reset();
		Character._GetHit+=GetHit;
		Character._ChangeHP+=ShowChangeHPBar;
		Character._Die+=Die;
		Character._ChangeWP+=ShowChangeWPBar;
	}
	public virtual void TurnOffBattle(){
		MoveUsed = null;
		HoldingMove = false;
		ClearTimers();
		selectActions.clearAll();
		Character._GetHit-=GetHit;
		Character._Die-=Die;
		Character._ChangeHP-=ShowChangeHPBar;
		Character._ChangeWP-=ShowChangeWPBar;
	}
	public virtual void ReturnToOverworld(){
		ProcessMode=ProcessModeEnum.Disabled;
		TurnOffBattle();
		AnimatorTree.Active=false;
		AnimatorTree.Set("parameters/conditions/Ended",false);
		Hide();
		Overworld.BattleEnd();
	}
	public void SetOffsets(){
		HPBar.GetParent<Node2D>().Position += Character.Base.BattleOffset;
		if(Character.isControlledByPlayer){
			EXPBar.GetParent<Node2D>().Position += Character.Base.BattleOffset;
			WPBar.GetParent<Node2D>().Position += Character.Base.BattleOffset;
		}
	}
}

using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class SelectActions : Node2D
{
	public enum SelectState{
		SelectAction,
		SelectMove,
		SelectItem,
		SelectUser,
		SelectTarget,
	}
	[Export]public BattleCharacter character;
	[Export]public AnimationTree Animator;
	
	Array<CharacterButtons> SelectedButtons=new Array<CharacterButtons>(),CurrentButtons=new Array<CharacterButtons>();

	SelectState currentState,MoveOrItem;
	Array<SelectState> previousState=new Array<SelectState>();
	Moves move;
	int statePointer, charactersSelected, possibleAmount;

	public bool animating;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	public void Start(){
		Animator.Set("parameters/conditions/Start",true);
		Animator.Set("parameters/conditions/Choice",false);
		Animator.Active=true;
		FlipText();
		Show();
		WaitForAnimation();
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch (currentState){
			case SelectState.SelectAction:
				if(!animating){
					if(Input.IsActionJustPressed("Move")){
						Animator.Set("parameters/conditions/Start",false);
						Animator.Set("parameters/conditions/Choice",true);
						Animator.Set("parameters/Choice/blend_position",0);
						previousState.Add(currentState);
						ActivateState(SelectState.SelectMove);
						MoveOrItem=SelectState.SelectMove;
						WaitForAnimation();
					}
					if(Input.IsActionJustPressed("Item")){
						Animator.Set("parameters/conditions/Start",false);
						Animator.Set("parameters/conditions/Choice",true);
						Animator.Set("parameters/Choice/blend_position",1);
						previousState.Add(currentState);
						ActivateState(SelectState.SelectItem);
						MoveOrItem=SelectState.SelectItem;
						WaitForAnimation();
					}
					if(Input.IsActionJustPressed("Run")){
						Animator.Set("parameters/conditions/Start",false);
						Animator.Set("parameters/conditions/Choice",true);
						Animator.Set("parameters/Choice/blend_position",2);
						BattleManager.instance.Run();
					}
				}
			break;
			case SelectState.SelectMove:
				if(!animating){
					if(Input.IsActionJustPressed("Confirm")){
						move=BattleManager.instance.CurrentMoveButton.move;
						character.MoveUsed=move;
						if(move.Base.UserAmount>1){
							previousState.Add(currentState);
							ActivateState(SelectState.SelectUser);
						}
						else{
							previousState.Add(currentState);
							ActivateState(SelectState.SelectTarget);
						}
					}
					if(Input.IsActionJustPressed("Deny")){
						ReturnToPreviousState();
					}					
				}
			break;
			case SelectState.SelectItem:
				if(!animating){
					if(Input.IsActionJustPressed("Confirm")){
						move=BattleManager.instance.CurrentItemButton.item.moves;
						character.MoveUsed=move;
						if(move.Base.UserAmount>1){
							previousState.Add(currentState);
							ActivateState(SelectState.SelectUser);
						}
						else{
							previousState.Add(currentState);
							ActivateState(SelectState.SelectTarget);
						}
					}
					
					if(Input.IsActionJustPressed("Deny")){
						ReturnToPreviousState();
					}					
				}
			break;
			case SelectState.SelectUser:
					if(Input.IsActionJustPressed("Confirm")){
						AddCharacter(BattleManager.instance.UserCharacters,BattleManager.instance.CurrentCharacterButton);
						if(charactersSelected==move.Base.UserAmount-1||charactersSelected==possibleAmount){
							previousState.Add(currentState);
							ActivateState(SelectState.SelectTarget);
						}			

					}
					if(Input.IsActionJustPressed("Deny")){
						if(charactersSelected==0){
							ReturnToPreviousState();
						}
						else{
							RemoveCharacter(BattleManager.instance.UserCharacters,SelectedButtons[SelectedButtons.Count-1]);
						}
					}
			break;
			case SelectState.SelectTarget:
					if(Input.IsActionJustPressed("Confirm")){
						AddCharacter(BattleManager.instance.TargetCharacters,BattleManager.instance.CurrentCharacterButton);
						if(charactersSelected==move.Base.TargetAmount||charactersSelected==possibleAmount){
							character.UseMove(move);
							clearAll();

						}
					}
					if(Input.IsActionJustPressed("Deny")){
						if(charactersSelected==0){
							ReturnToPreviousState();
						}
						else{
							RemoveCharacter(BattleManager.instance.TargetCharacters,SelectedButtons[SelectedButtons.Count-1]);
						}
					}
			break;
		}


	}
	void ReturnToPreviousState(){
			ActivateState(previousState[previousState.Count-1]);
			previousState.RemoveAt(previousState.Count-1);
	}
	void ActivateState(SelectState state){
		currentState=state;
		clearMost();
			switch (currentState){
			case SelectState.SelectAction:
					WaitForAnimation();
					Animator.Set("parameters/conditions/Start",true);
					Animator.Set("parameters/conditions/Choice",false);
			break;
			case SelectState.SelectMove:
				BattleManager.instance.moveList.Show();

				BattleManager.instance.moveList.FillButtons(BattleManager.instance.moveList.pointerStart,ScrollList.StartEnd.Regular);

			break;
			case SelectState.SelectItem:
				BattleManager.instance.itemList.Show();
				BattleManager.instance.itemList.FillButtons(BattleManager.instance.moveList.pointerStart,ScrollList.StartEnd.Regular);
			break;
			case SelectState.SelectUser:
				for(int i=0;i<character.PartyButtons.Count;i++){
					if(character.PartyButtons[i].Character!=character&&character.PartyButtons[i].Character.Character.status!=Character.Status.KO){
						character.PartyButtons[i].Show();
						character.PartyButtons[i].GrabFocus();
					}
				}
			break;
			case SelectState.SelectTarget:
					if(move.Base.TargetsParty){
						for(int i=0;i<character.PartyButtons.Count;i++){
							if(character.PartyButtons[i].Character.Character.status!=Character.Status.KO && move.Base.type!=MoveBase.Type.Revive){
								character.PartyButtons[i].Show();
								possibleAmount++;
								CurrentButtons.Add(character.PartyButtons[i]);
								character.PartyButtons[i].GrabFocus();
							}
							else if(character.PartyButtons[i].Character.Character.status==Character.Status.KO && move.Base.type==MoveBase.Type.Revive){
								character.PartyButtons[i].Show();
								possibleAmount++;
								CurrentButtons.Add(character.PartyButtons[i]);
								character.PartyButtons[i].GrabFocus();
							}
						}
					}
					else{
						for(int i=0;i<character.EnemyButtons.Count;i++){
							if(character.EnemyButtons[i].Character.Character.status!=Character.Status.KO){
								character.EnemyButtons[i].Show();
								possibleAmount++;
								CurrentButtons.Add(character.EnemyButtons[i]);
														character.EnemyButtons[i].GrabFocus();
							}		
						}
					}
			break;
		}
	}
	void hideButtons(){
		for(int i=0;i<character.PartyButtons.Count;i++){
			character.PartyButtons[i].Hide();
		}
		for(int i=0;i<character.EnemyButtons.Count;i++){			
			character.EnemyButtons[i].Hide();
		}	
	}
	void AddCharacter(Array<BattleCharacter> List,CharacterButtons button){

		List.Add(button.Character);
		charactersSelected++;
		SelectedButtons.Add(button);
		button.Hide();	
		ReFocus(CurrentButtons);
	}
	void ReFocus(Array<CharacterButtons> List){
		for(int i=0;i<List.Count;i++){
			if(!SelectedButtons.Contains(List[i])){
				List[i].GrabFocus();
			}
		}
	}
	void RemoveCharacter(Array<BattleCharacter> List,CharacterButtons button){
		List.Remove(button.Character);
		charactersSelected--;
		SelectedButtons.Remove(button);
		button.Show();
		button.GrabFocus();
	}
	void clearMost(){
		possibleAmount=0;
		charactersSelected=0;
		CurrentButtons.Clear();
		SelectedButtons.Clear();
		BattleManager.instance.CurrentCharacterButton=null;
		BattleManager.instance.CurrentItemButton=null;
		BattleManager.instance.CurrentMoveButton=null;

		BattleManager.instance.moveList.ClearAll();
		BattleManager.instance.itemList.ClearAll();
		BattleManager.instance.moveList.Hide();
		BattleManager.instance.itemList.Hide();

		hideButtons();
	}
	public void clearAll(){
		clearMost();
		BattleManager.instance.moveList.character=null;
		BattleManager.instance.itemList.character=null;
		previousState.Clear();
		Animator.Active=false;
		Hide();	
		currentState=SelectState.SelectAction;
		MoveOrItem=SelectState.SelectAction;
		move=null;
		ProcessMode=ProcessModeEnum.Disabled;
	}
	void WaitForAnimation(){
		animating=true;
		void SetAnimationFalse(){
			animating=false;
		}
		Tween tween=CreateTween();
		tween.TweenInterval(0.5);
		tween.TweenCallback(Callable.From(SetAnimationFalse));
		tween.Finished+=tween.Kill;
	}
	void FlipText(){
		Array<Node>Labels=GetChildren();
		for(int i=0;i<Labels.Count;i++){
			if(Labels[i].GetType()==typeof(TextureRect)){
				Node2D Text=Labels[i].GetChild<Node2D>(0);
				float NormalizedX=character.GlobalScale.X/Mathf.Abs(character.GlobalScale.X);
				float NormalizedY=character.GlobalScale.Y/Mathf.Abs(character.GlobalScale.Y);
				Text.Scale=new Vector2(NormalizedX,NormalizedY);
				Text.Rotation=character.GlobalRotation;
				RichTextLabel KeyButton=Text.GetChild<RichTextLabel>(1);
				if(NormalizedY==-1){
					KeyButton.Position=new Vector2(12,KeyButton.Position.Y);
				}
				else{
					KeyButton.Position=new Vector2(-26.9f,KeyButton.Position.Y);
				}
			}
		}	
	}
}

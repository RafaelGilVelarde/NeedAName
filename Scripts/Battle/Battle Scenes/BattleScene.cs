using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
[GlobalClass]
public partial class BattleScene : Resource
{
    DialogicCSharp Dialog;
    BattleManager Battle;
    Callable endDialogue;
    bool DialogueFlag;
    public virtual void StartBattleEffect(){
        Dialog=DialogicCSharp.instance;
        Battle=BattleManager.instance;
        endDialogue=new Callable(this,MethodName.EndDialogue);
    }
    public virtual void StartRoundEffect(){

    }
    public virtual void StartTurnEffect(bool first){
        /*if(first==true){
            Stats stats=BattleManager.instance.EnemyParty[0].Character.stats;
            if(stats.MaxHP>stats.HP &&!DialogueFlag){
                Battle.CanStartTurn=false;
                Dialog.DialogicRoot.Connect("signal_event",endDialogue);
                DialogueFlag=true;
                Dialog.StartDialogue("BattleTest",false);
            }
        }*/
    }
    public virtual void EndBattleEffect(){

    }
    public virtual void BattleEnd(){
        BattleManager.instance.ReturnToOverworld();
    }
    public virtual void ReturnToOverworld(){
        BattleManager Battle=BattleManager.instance;
        BattleState State=Battle.State;
        for(int i=0;i<Battle.EnemyParty.Count;i++){
            BattleCharacter Enemy=Battle.EnemyParty[i];
            Enemy.Character.stats.HP=Enemy.Character.stats.MaxHP;
            Enemy.Character.status=Character.Status.Normal;
        }
        switch (State){
            case BattleState.Win:
                AwardExp();
                for(int i=0;i<Battle.Party.Count;i++){
                    Battle.Party[i].Character.status=Character.Status.Normal;
                    Battle.Party[i].Reset();
                }
                for(int i=0;i<Battle.EnemyParty.Count;i++){
                    CharacterBody2D Enemy=Battle.EnemyParty[i].GetParent<CharacterBody2D>();
                    Tween End=Enemy.CreateTween();
                    End.TweenProperty(Enemy,"modulate:a",0,0.5f);
                    End.Finished+=Enemy.QueueFree;
                    End.Finished+=End.Kill;
                }
            break;
            case BattleState.Lose:
                for(int i=0;i<Battle.Party.Count;i++){
                    Battle.Party[i].Character.stats.HP=Battle.Party[i].Character.stats.MaxHP;
                    Battle.Party[i].Character.status=Character.Status.Normal;
                    Battle.Party[i].Reset();

                }
                if(Battle.EnemyParty.Count>1){
                    Battle.EnemyParty[0].Reset();
                    for(int i=1;i<Battle.EnemyParty.Count;i++){
                        Tween End=Battle.EnemyParty[i].CreateTween();
                        CharacterBody2D Enemy=Battle.EnemyParty[i].GetParent<CharacterBody2D>();
                        End.TweenProperty(Enemy,"modulate:a",0,0.5f);
                        End.Finished+=Enemy.QueueFree;
                        End.Finished+=End.Kill;
                    }
                }
            break;
            case BattleState.Run:
                for(int i=0;i<Battle.Party.Count;i++){
                    Battle.Party[i].Character.status=Character.Status.Normal;
                    Battle.Party[i].Reset();
                }
                if(Battle.EnemyParty.Count>1){
                    Battle.EnemyParty[0].Reset();
                    for(int i=1;i<Battle.EnemyParty.Count;i++){
                        Tween End=Battle.EnemyParty[i].CreateTween();
                        CharacterBody2D Enemy=Battle.EnemyParty[i].GetParent<CharacterBody2D>();
                        End.TweenProperty(Enemy,"modulate:a",0,0.5f);
                        End.Finished+=Enemy.QueueFree;
                        End.Finished+=End.Kill;
                    }
                }
            break;
        }
    }
    public virtual Vector2 GetCenterView(Array<Vector2> Party, Array<Vector2>Enemy){
        float xMin=Mathf.Inf;
        float yMin=Mathf.Inf;
        float xMax=-Mathf.Inf;
        float yMax=-Mathf.Inf;
        for(int i=0;i<Party.Count;i++){
            if(Party[i].X>xMax){
                xMax=Party[i].X;
            }
            if(Party[i].Y>yMax){
                yMax=Party[i].Y;
            }
            if(Party[i].X<xMin){
                xMin=Party[i].X;
            }
            if(Party[i].Y<yMin){
                yMin=Party[i].Y;
            }

        }

        for(int i=0;i<Enemy.Count;i++){
            if(Enemy[i].X>xMax){
                xMax=Enemy[i].X;
            }
            if(Enemy[i].Y>yMax){
                yMax=Enemy[i].Y;
            }
            if(Enemy[i].X<xMin){
                xMin=Enemy[i].X;
            }
            if(Enemy[i].Y<yMin){
                yMin=Enemy[i].Y;
            }
        }
        Vector2 Result=new Vector2((xMin+xMax)/2,(yMin+yMax)/2);
        Debug.WriteLine("Result: x: "+xMin+","+xMax+","+"y: "+yMin+","+yMax+"Final: "+Result);
        return Result;
    }
    public virtual void EndDialogue(string argument){
        if(argument=="End"){
            Battle.CanStartTurn=true;
            Battle.StartTurn(false);
            Dialog.DialogicRoot.Disconnect("signal_event",endDialogue);
        }
    }
    public virtual void AwardExp(){
        BattleManager Battle=BattleManager.instance;
        int[] EXP = new int[Battle.Party.Count];
        for(int i=0;i<Battle.EnemyParty.Count;i++){
            Character EnemyCharacter=Battle.EnemyParty[i].Character;
            float ExpYield = ((EnemyCharacterBase)EnemyCharacter.Base).ExpYield;
            for(int j=0;j<Battle.Party.Count;j++){
                PartyCharacters PartyCharacter=(PartyCharacters)Battle.Party[j].Character;
                int EXPAwarded = (int)(EnemyCharacter.stats.Lv*ExpYield*Mathf.Pow(2,EnemyCharacter.stats.Lv/PartyCharacter.stats.Lv)/Battle.Party.Count);
                EXP[j] += EXPAwarded;
            }        
        }
        for(int i = 0;i<Battle.Party.Count;i++){
            PartyCharacters PartyCharacter=(PartyCharacters)Battle.Party[i].Character;        
            PartyCharacter.GainExp(EXP[i]);
        }
    }
}

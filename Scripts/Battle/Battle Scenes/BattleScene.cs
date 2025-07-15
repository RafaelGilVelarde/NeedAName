using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
public enum TimelineType{
    None,
    StartTurn,
    Move,
    EndBattle
}

[GlobalClass]
public partial class BattleScene : Resource
{
    protected DialogicCSharp Dialog;
    protected BattleManager Battle;
    protected Callable endDialogue, beginCutscene, endCutscene;
    protected TimelineType timelineType;
    [Export] public string WinTimeline = "Win";
    public bool DialogueFlag;
    [Export] public float EscapeChance = 100;
    [Export] public bool Horizontal, Cutscene;
    [Export] public Array<float> PartyFloorY, EnemyFloorY;
    protected CutscenePlayer CutsceneAnimator;
    public virtual void StartBattleEffect()
    {

        Dialog = DialogicCSharp.instance;
        Battle = BattleManager.instance;
        endDialogue = new Callable(this, MethodName.EndDialogue);
        Dialog?.DialogicRoot?.Connect("timeline_ended", endDialogue);
        
        if (GameManager.Instance.CurrentScene.CutsceneAnimator != null && Cutscene)
        {
            CutsceneAnimator = GameManager.Instance.CurrentScene.CutsceneAnimator;
            beginCutscene = new Callable(this, MethodName.StartCutscene);
            endCutscene = new Callable(this, MethodName.EndCutscene);
            Dialog?.DialogicRoot?.Connect("signal_event", beginCutscene);
            Dialog?.DialogicRoot?.Connect("signal_event", endCutscene);
        }
    }
    public virtual void StartRoundEffect()
    {

    }
    public virtual void StartTurnEffect()
    {

    }
    public virtual void EndBattleEffect()
    {

    }
    public virtual void BattleEnd()
    {
        BattleState State = Battle.State;
        switch (State)
        {
            case BattleState.Win:
                AwardExp();
                StartDialogue(WinTimeline, true, false, TimelineType.EndBattle);
                //BattleManager.instance.ReturnToOverworld();
                break;
            case BattleState.Lose:
                BattleManager.instance.OpenLossScreen();
                for (int i = 0; i < Battle.Party.Count; i++)
                {
                    //Battle.Party[i].Character.stats.HP=Battle.Party[i].Character.TotalStats.MaxHP;
                    Battle.Party[i].Character.status = Character.Status.Normal;
                    Battle.Party[i].TurnOffBattle();

                }
                /*if(Battle.EnemyParty.Count>1){
                    Battle.EnemyParty[0].Reset();
                    for(int i=1;i<Battle.EnemyParty.Count;i++){
                        Tween End=Battle.CreateTween();
                        CharacterBody2D Enemy=Battle.EnemyParty[i].GetParent<CharacterBody2D>();
                        End.TweenProperty(Enemy,"modulate:a",0,0.5f);
                        End.Finished+=Enemy.QueueFree;
                        End.Finished+=End.Kill;
                    }
                }*/
                for (int i = 0; i < Battle.EnemyParty.Count; i++)
                {
                    Battle.EnemyParty[i].Character.stats.HP = Battle.EnemyParty[i].Character.TotalStats.MaxHP;
                    Battle.EnemyParty[i].Character.status = Character.Status.Normal;
                    Battle.EnemyParty[i].TurnOffBattle();
                }
                break;
            case BattleState.Run:
                BattleManager.instance.ReturnToOverworld();
                break;
        }

    }
    public virtual void ReturnToOverworld()
    {
        Dialog?.DialogicRoot?.Disconnect("timeline_ended", endDialogue);
        if (Cutscene)
        {
            Dialog?.DialogicRoot?.Disconnect("signal_event", beginCutscene);
            Dialog?.DialogicRoot?.Disconnect("signal_event", endCutscene);            
        }
        BattleManager Battle = BattleManager.instance;
        BattleState State = Battle.State;
        for (int i = 0; i < Battle.EnemyParty.Count; i++)
        {
            BattleCharacter Enemy = Battle.EnemyParty[i];
            Enemy.Character.stats.HP = Enemy.Character.TotalStats.MaxHP;
            Enemy.Character.status = Character.Status.Normal;
        }
        switch (State)
        {
            case BattleState.Win:
                for (int i = 0; i < Battle.Party.Count; i++)
                {
                    Battle.Party[i].Character.status = Character.Status.Normal;
                    Battle.Party[i].Reset();
                    Battle.Party[i].OriginPos = Vector2.Zero;
                    Battle.Party[i].HideEXPBar();
                    Battle.Party[i].ReturnToOverworld();
                }
                for (int i = 0; i < Battle.EnemyParty.Count; i++)
                {
                    /*Battle.EnemyParty[i].OriginPos=Vector2.Zero;
			        Battle.EnemyParty[i].ReturnToOverworld();*/
                    CharacterBody2D Enemy = Battle.EnemyParty[i].GetParent<CharacterBody2D>();
                    Tween End = Battle.CreateTween();
                    End.TweenProperty(Enemy, "modulate:a", 0, 0.5f);
                    End.Finished += Enemy.QueueFree;
                    End.Finished += End.Kill;
                }
                break;
            case BattleState.Lose:

                break;
            case BattleState.Run:
                for (int i = 0; i < Battle.Party.Count; i++)
                {
                    Battle.Party[i].Character.status = Character.Status.Normal;
                    Battle.Party[i].Reset();
                    Battle.Party[i].OriginPos = Vector2.Zero;
                    Battle.Party[i].ReturnToOverworld();
                }
                Battle.EnemyParty[0].Reset();
                Battle.EnemyParty[0].OriginPos = Vector2.Zero;
                Battle.EnemyParty[0].ReturnToOverworld();
                if (Battle.EnemyParty.Count > 1)
                {
                    for (int i = 1; i < Battle.EnemyParty.Count; i++)
                    {
                        Tween End = Battle.CreateTween();
                        CharacterBody2D Enemy = Battle.EnemyParty[i].GetParent<CharacterBody2D>();
                        End.TweenProperty(Enemy, "modulate:a", 0, 0.5f);
                        //End.Play();
                        End.Finished += Enemy.QueueFree;
                        End.Finished += End.Kill;
                    }
                }
                break;
        }
    }
    public virtual Vector2 GetCenterView(Array<Vector2> Party, Array<Vector2> Enemy)
    {
        float xMin = Mathf.Inf;
        float yMin = Mathf.Inf;
        float xMax = -Mathf.Inf;
        float yMax = -Mathf.Inf;
        for (int i = 0; i < Party.Count; i++)
        {
            if (Party[i].X > xMax)
            {
                xMax = Party[i].X;
            }
            if (Party[i].Y > yMax)
            {
                yMax = Party[i].Y;
            }
            if (Party[i].X < xMin)
            {
                xMin = Party[i].X;
            }
            if (Party[i].Y < yMin)
            {
                yMin = Party[i].Y;
            }

        }

        for (int i = 0; i < Enemy.Count; i++)
        {
            if (Enemy[i].X > xMax)
            {
                xMax = Enemy[i].X;
            }
            if (Enemy[i].Y > yMax)
            {
                yMax = Enemy[i].Y;
            }
            if (Enemy[i].X < xMin)
            {
                xMin = Enemy[i].X;
            }
            if (Enemy[i].Y < yMin)
            {
                yMin = Enemy[i].Y;
            }
        }
        Vector2 Result = new Vector2((xMin + xMax) / 2, (yMin + yMax) / 2);
        Debug.WriteLine("View: " + Result);
        return Result;
    }

    public void StartDialogue(string timeline, bool Pause, bool Auto, TimelineType type)
    {
        timelineType = type;
        Dialog.StartDialogue(timeline, Pause, Auto);
        if (Auto)
        {
            Dialog.CallDeferred("AutoAdvance", true, false);
        }
    }
    public virtual void EndDialogue()
    {
        //if(argument=="End"){
        Debug.WriteLine("Ended dialogue, type: " + timelineType);
        switch (timelineType)
        {
            case TimelineType.StartTurn:
                Battle.CanStartTurn = true;
                SceneTreeTimer timer = Battle.GetTree().CreateTimer(0.2);
                timer.Timeout += () =>
                Battle.StartTurn();
                break;
            case TimelineType.Move:
                break;
            case TimelineType.EndBattle:
                BattleManager.instance.ReturnToOverworld();
                break;
        }
        timelineType = TimelineType.None;
        //}
    }

    public virtual void AwardExp()
    {
        BattleManager Battle = BattleManager.instance;
        int[] EXP = new int[Battle.Party.Count];
        for (int i = 0; i < Battle.EnemyParty.Count; i++)
        {
            Character EnemyCharacter = Battle.EnemyParty[i].Character;
            float ExpYield = ((EnemyCharacterBase)EnemyCharacter.Base).ExpYield;
            for (int j = 0; j < Battle.Party.Count; j++)
            {
                PartyCharacters PartyCharacter = (PartyCharacters)Battle.Party[j].Character;
                int EXPAwarded = (int)(EnemyCharacter.stats.Lv * ExpYield * Mathf.Pow(2, EnemyCharacter.stats.Lv / PartyCharacter.stats.Lv) / Battle.Party.Count);
                EXP[j] += EXPAwarded;
            }
        }
        for (int i = 0; i < Battle.Party.Count; i++)
        {
            PartyCharacters PartyCharacter = (PartyCharacters)Battle.Party[i].Character;
            PartyCharacterBase CharBase = (PartyCharacterBase)Battle.Party[i].Character.Base;
            int Level = PartyCharacter.stats.Lv;
            Battle.Party[i].ShowEXPBar(PartyCharacter.Exp, PartyCharacter.Exp + EXP[i], CharBase.ExpForLevel[Level] - CharBase.ExpForLevel[Level - 1], Level);
            PartyCharacter.GainExp(EXP[i]);
        }
    }
    
    protected virtual void StartCutscene(string Cutscene)
    {
        if (CutsceneAnimator.CutsceneNames.Contains(Cutscene))
        {
            CutsceneAnimator.PlayAnimation(Cutscene);
        }
    }
    protected virtual void EndCutscene(string Argument)
    {
        if (Argument == "EndCutscene")
        {
            CutsceneAnimator.EndAnimation();
        }
    }
}

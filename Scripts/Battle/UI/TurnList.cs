using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;

public partial class TurnList : Control
{
    [Export] Vector2 HorizontalLocation, VerticalLocation;
    [Export] Array<TurnBox> Turns, TurnAux;
    [Export] Array<Vector2> TurnPositions, TurnAuxPositions;
    [Export] float Separation = 64;
    [Export] Color Neutral, Current, AuxColor;
    [Export] bool isVertical = true;
    [Export] NinePatchRect MainContainer;

    Tween MainTween;

    public override void _Ready()
    {
        base._Ready();
        for(int i = 0; i < GetChildCount()-1; i++)
        {
            if (i%2 == 0)
            {
                Turns.Add(GetChild<TurnBox>(i));
                Vector2 Pos = new Vector2(Separation*(Turns.Count-1),Separation*(Turns.Count-1));
                TurnPositions.Add(Pos);
            }
            else
            {
                TurnAux.Add(GetChild<TurnBox>(i));
                Vector2 Pos= new Vector2(Separation*(Turns.Count-1),Separation*TurnAux.Count);
                TurnAuxPositions.Add(Pos);
            }
            GetChild<TurnBox>(i).Hide();
        }

        for(int i = 0; i < TurnAux.Count; i++)
        {
            TurnAux[i].CharacterSprite.Modulate = AuxColor;

        }
    }
    public void Start(Array<BattleCharacter> Characters)
    {
        Show();
        for(int i = 0; i < Characters.Count; i++)
        {
            Turns[i].Show();
            Turns[i].Animator.Play("start");
            Turns[i].CurrentCharacter = Characters[i];
            //Turns[i].ActiveTime.Text = Turns[i].CurrentCharacter.CurrentDelay.ToString();
            Turns[i].CharacterSprite.Texture = (Texture2D)Turns[i].CurrentCharacter.Character.Base.BattleIcon;
        }
        for(int i = Characters.Count; i < Turns.Count; i++)
        {
            Turns[i].Hide();
        }
        Tween ContainerTween = CreateTween();
        if (isVertical)
        {
            MainContainer.Size = new Vector2(81,0);
            ContainerTween.TweenProperty(MainContainer,"size",new Vector2(81,567),0.4);
        }
        else
        {
            MainContainer.Size = new Vector2(0,81);
            ContainerTween.TweenProperty(MainContainer,"size",new Vector2(567,81),0.4);
        }
        Turns[0].CharacterSprite.Modulate = Current;
    }
    public void ChooseLocation(bool Vertical)
    {
        if (Vertical)
        {
            isVertical = true;
            Position = VerticalLocation;
        }
        else
        {
            isVertical = false;
            Position = HorizontalLocation;
        }
    }

    public void UpdateTurns(Array<BattleCharacter> Characters)
    {

        RestartTween(MainTween);
        MainTween = CreateTween();
        MainTween.SetProcessMode(Tween.TweenProcessMode.Physics);
        MainTween.SetParallel(true);
        //MainTween.TweenProperty(Turns[0],"modulate:a",0,0.3);
        Array<TurnBox> AuxArray = Turns.Duplicate();
        Array<TurnBox> Dispose = new Array<TurnBox>();
        for(int i = 0; i < Turns.Count; i++)
        {
            int CharIndex = Characters.IndexOf(Turns[i].CurrentCharacter);
            if (CharIndex >= 0)
            {
                if (isVertical)
                {
                    MainTween.TweenProperty(Turns[i],"position",new Vector2(Turns[i].Position.X,TurnPositions[CharIndex].Y),0.2);                                                
                }
                else
                {
                    MainTween.TweenProperty(Turns[i],"position",new Vector2(TurnPositions[CharIndex].X,Turns[i].Position.Y),0.2);                                                
                }
            }
            else if (Turns[i].CurrentCharacter!= null)
            {

                Dispose.Add(Turns[i]);
                Turns[i].Animator.Play("end");
                Turns[i].CurrentCharacter = null;
            }

        }
            for(int i = 0;i<Characters.Count;i++)
            {
                int chara = -1;
                for(int j = 0; j < AuxArray.Count; j++)
                {
                    if (AuxArray[j].CurrentCharacter != null)
                    {
                        if(AuxArray[j].CurrentCharacter == Characters[i])
                        {
                            chara = j;
                        }
                    }
                }
                Control Aux = Turns[i];
                if (chara >= 0)
                {

                    Turns[i] = AuxArray[chara];
                    Turns[i].Show();
                    //Turns[i].ActiveTime.Text = Turns[i].CurrentCharacter.CurrentDelay.ToString();
                    Turns[i].CharacterSprite.Modulate = Neutral; 
                                   
                }
            }  
        for(int i = 0; i < Dispose.Count; i++)
        {
            Turns[i+Characters.Count] = Dispose[i];
        }

        Turns[0].CharacterSprite.Modulate = Current;
        MainTween.Finished += () =>
        {
            Turns[Characters.Count-1].Animator.Play("start");
            HideAuxTurns();
        };
        for(int i = 0; i < TurnAux.Count; i++)
        {
            TurnAux[i].Animator.Play("end");
        }
    }
    public void UpdateAuxTurns(Moves move, BattleCharacter AddUser = null, BattleCharacter AddTarget = null)
    {
       Array<int> indexes = new Array<int>();
        Array<BattleCharacter> TurnOrder= BattleManager.instance.TurnOrder.Duplicate();
        Array<BattleCharacter> TotalCharacters= new Array<BattleCharacter>();

        foreach (BattleCharacter chara in BattleManager.instance.UserCharacters)
		{
			chara.AuxDelay =  move.Base.Delay/chara.Character.stats.Speed;
            TotalCharacters.Add(chara);
		}
        if (AddUser != null)
        {
            AddUser.AuxDelay =  move.Base.Delay/AddUser.Character.stats.Speed;
            TotalCharacters.Add(AddUser);
        }

        if (move.Base.IsClass(nameof(RaiseStatMove)))
        {
            if (((RaiseStatMove)move.Base).Stats.DelayUp)
            {
                foreach(BattleCharacter chara in BattleManager.instance.TargetCharacters)
                {
                    chara.AuxDelay = ((RaiseStatMove)move.Base).Stats.DelayTarget + chara.CurrentDelay;
                    TotalCharacters.Add(chara);
                }                
                if (AddTarget != null)
                {
                    AddTarget.AuxDelay =  move.Base.Delay/AddTarget.Character.stats.Speed;
                    TotalCharacters.Add(AddTarget);
                }
            }
        }


        for(int i = 0; i < TurnOrder.Count; i++)
        {
            if (!TotalCharacters.Contains(TurnOrder[i]))
            {
                TurnOrder[i].AuxDelay = TurnOrder[i].CurrentDelay;
            }
        }
        IOrderedEnumerable<BattleCharacter> AuxOrder= TurnOrder.OrderBy(character=>character.AuxDelay);
        List<BattleCharacter> Aux = AuxOrder.ToList();

        
        for(int i = 0; i < TotalCharacters.Count(); i++)
        {
            indexes.Add(Aux.IndexOf(TotalCharacters[i]));
        }


        RestartTween(MainTween);
        MainTween = CreateTween();
        MainTween.SetProcessMode(Tween.TweenProcessMode.Physics);
        MainTween.SetParallel(true);
        for(int i = 0; i < Turns.Count(); i++)
        {
            int Mult = 0;
            for(int j = 0; j < indexes.Count(); j++)
            {
                if (i > indexes[j])
                {
                    Mult++;
                }
            }
            if (isVertical)
            {
                MainTween.TweenProperty(Turns[i],"position",new Vector2(Turns[i].Position.X,TurnPositions[i].Y+Separation*Mult),0.3);
                TurnAux[i].Position = new Vector2(TurnAux[i].Position.X,TurnAuxPositions[i].Y+Separation*Mult);
            }
            else
            {
                MainTween.TweenProperty(Turns[i],"position",new Vector2(TurnPositions[i].X+Separation*Mult,Turns[i].Position.Y),0.3);
                TurnAux[i].Position = new Vector2(TurnAuxPositions[i].X*Mult,TurnAux[i].Position.Y);
            }

        }
        for(int i = 0;i<indexes.Count;i++)
        {
            TurnAux[indexes[i]].CharacterSprite.Texture = (Texture2D)TotalCharacters[i].Character.Base.BattleIcon;
            TurnAux[indexes[i]].ActiveTime.Text = TotalCharacters[i].AuxDelay.ToString();
            TurnAux[indexes[i]].Animator.Play("start");
            TurnAux[indexes[i]].Show();
        }
        for(int i = 0; i < TurnAux.Count; i++)
        {
            if (!indexes.Contains(i))
            {
                TurnAux[i].Hide();
            }
        }
        for(int i = 0; i < TurnOrder.Count(); i++)
        {
            TurnOrder[i].AuxDelay = 0;
        }
    }
    void RestartTween(Tween tween)
    {
        if (tween != null)
        {
            tween.Stop();            
        }
    }

    public void HideAuxTurns()
    {
        RestartTween(MainTween);
        MainTween = CreateTween();
        MainTween.SetProcessMode(Tween.TweenProcessMode.Physics);
        MainTween.SetParallel(true);
        for(int i = 0; i < TurnAux.Count(); i++)
        {
            TurnAux[i].Hide();
            if (isVertical)
            {
                TurnAux[i].Position = new Vector2(TurnAux[i].Position.X,TurnAuxPositions[i].Y);                
            }
            else
            {
                TurnAux[i].Position = new Vector2(TurnAuxPositions[i].X,TurnAux[i].Position.Y);
            }
        }
        for(int i = 0; i < Turns.Count(); i++)
        {
            if (isVertical)
            {
                MainTween.TweenProperty(Turns[i],"position",new Vector2(Turns[i].Position.X,TurnPositions[i].Y),0.1);                            
            }
            else
            {
                MainTween.TweenProperty(Turns[i],"position",new Vector2(TurnPositions[i].X,Turns[i].Position.Y),0.1);                            
            }
            //Turns[i].Position = TurnPositions[i];
        }

    }
    public void ColorFocus(Color color, Array<BattleCharacter> characters)
    {
        for(int i = 0; i < Turns.Count; i++)
        {
            if (Turns[i] != null)
            {
                if(characters.Contains(Turns[i].CurrentCharacter))
                {
                    Turns[i].CharacterSprite.Modulate = color;            
                    
                }
            }
        }
    }

}

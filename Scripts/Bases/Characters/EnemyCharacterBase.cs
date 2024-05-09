using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
[GlobalClass]
public partial class EnemyCharacterBase : CharacterBase
{
    [Export]Array<SteeringBehaviours> SeekBehaviours,IdleBehaviours;
    [Export]float SteeringForce;
    public virtual Moves MoveChoosing(BattleCharacter Character){
        RandomNumberGenerator RNG=new RandomNumberGenerator();
        Moves move=Character.Character.Moves[RNG.RandiRange(0,Character.Character.Moves.Count-1)];
        return move;
    }
    public virtual void UserChoosing(){

    }
    public virtual void TargetChoosing(Moves move, BattleCharacter BattleCharacter){
        RandomNumberGenerator RNG=new RandomNumberGenerator();
        if(!move.Base.TargetsParty){
            for(int i=0;i<move.Base.TargetAmount;i++){
                BattleCharacter target=BattleCharacter.EnemyParty[RNG.RandiRange(0,BattleCharacter.EnemyParty.Count-1)];
                if(!BattleManager.instance.TargetCharacters.Contains(target)){
                    BattleManager.instance.TargetCharacters.Add(target);
                }
            }
        }
        else{
            for(int i=0;i<move.Base.TargetAmount;i++){
                BattleCharacter target=BattleCharacter.ThisParty[(int)RNG.RandiRange(0,BattleCharacter.ThisParty.Count-1)];
                if(!BattleManager.instance.TargetCharacters.Contains(target)){
                    BattleManager.instance.TargetCharacters.Add(target);
                }
            }
        }

    }
    public virtual (Vector2,Vector2,int) Moving(EnemyOverworldController controller){
        Vector2 Direction=Vector2.Zero;
        TargetBehaviours aux= (TargetBehaviours)SeekBehaviours[1];
        aux.FindOpponent(controller);
        int Max=0;
        float[] danger = new float[8];
        float[] interest = new float[8];
        float[] Substraction = new float[8];
        switch(controller.state){
            case MoveState.Idle:
                    foreach(SteeringBehaviours behavior in IdleBehaviours)
                    {
                        (danger, interest) = behavior.GetSteering(danger, interest, controller);
                    }
                    for(int i = 0; i < 8; i++)
                    {
                        Substraction[i] =Mathf.Max(interest[i] - danger[i],0);
                        Substraction[i]= (float)Math.Round(Substraction[i],2);
                        //Debug.WriteLine("Interest "+i+ ":"+interest[i]);
                        /*if(Substraction[i]>Substraction[Max]+0.1){
                            Max=i;
                        }*/
                        Direction+=Directions.directions[i]*Substraction[i];
                    }
            break;
            case MoveState.Seek:
                    foreach(SteeringBehaviours behavior in SeekBehaviours)
                    {
                        (danger, interest) = behavior.GetSteering(danger, interest, controller);
                    }
                    for(int i = 0; i < 8; i++)
                    {
                        Substraction[i] =Mathf.Max(interest[i] - danger[i],0);
                        Substraction[i]= (float)Math.Round(Substraction[i],2);
                        //Debug.WriteLine("Interest "+i+ ":"+interest[i]);
                        /*if(Substraction[i]>Substraction[Max]+0.1){
                            Max=i;
                        }*/
                        Direction+=Directions.directions[i]*Substraction[i];
                    }
                    //Debug.WriteLine("Max: "+Max);
                    //Direction=Directions.directions[Max];       
                    //Debug.WriteLine(Direction);           

            break;
            case MoveState.Run:
            break;
        }
        return (Direction*SteeringForce*controller.Speed-controller.Parent.Velocity,Direction,Max);    
    }
}

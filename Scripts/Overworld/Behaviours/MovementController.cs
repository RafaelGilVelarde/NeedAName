using Godot;
using Godot.Collections;
using System;
[GlobalClass]

public partial class MovementController : Resource
{

    [Export]float SteeringForce;
    [Export]Array<SteeringBehaviours> SeekBehaviours,IdleBehaviours;

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

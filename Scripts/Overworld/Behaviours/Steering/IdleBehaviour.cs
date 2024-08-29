using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
[GlobalClass]
public partial class IdleBehaviour : SteeringBehaviours
{
    [Export]private float ReachThreshold;
    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, EnemyOverworldController enemy)
    {
        FindOpponent(enemy);
            float Direction;

            Vector2 TargetDirection = enemy.CachedTargetPosition - (Vector2)enemy.GlobalPosition;
            
            Direction = TargetDirection.Length();
            if(Direction<ReachThreshold){
                RandomNumberGenerator RNG=new RandomNumberGenerator();
                enemy.CurrentIdleTarget=RNG.RandiRange(0,enemy.IdleTargets.Count-1);
            }
            else{
                for (int j = 0; j < interest.Length; j++)
                {
                    float result = TargetDirection.Normalized().Dot(Directions.directions[j]);
                    if (result > 0)
                    {
                        float value = result;
                        if (Mathf.Abs(value) > interest[j])
                        {
                            interest[j] = value /* (InterestVariable-Direction)/InterestVariable *InterestVariable *//Direction*400;
                            /*if(j!=enemy.DirectionIndex){
                                interest[j]*=0.5f;
                            }*/
                        }
                    }

                }

            }
            return (danger, interest);    
    }
    public void FindOpponent(EnemyOverworldController enemy){
            var spaceState = enemy.GetWorld2D().DirectSpaceState;
            var query = PhysicsRayQueryParameters2D.Create(enemy.GlobalPosition, enemy.IdleTargets[enemy.CurrentIdleTarget].GlobalPosition, enemy.Parent.CollisionMask);
            query.Exclude=new Godot.Collections.Array<Rid> {enemy.Parent.GetRid()};
            query.CollideWithAreas=true;
            var result = spaceState.IntersectRay(query);
            if(result.Count>0){
                if(result["collider"].As<Node2D>()==enemy.IdleTargets[enemy.CurrentIdleTarget].GetChild<Node2D>(0)){
                    /*
                    Debug.WriteLine("This Position: "+enemy.GlobalPosition);
                    Debug.WriteLine("Origin: "+enemy.Transform.Origin);*/
                    enemy.CachedTargetPosition=enemy.IdleTargets[enemy.CurrentIdleTarget].GlobalPosition;
                }
                /*else{
                    Debug.WriteLine("Cache: "+CachedTargetPosition);
                }*/
            }
            enemy.RaycastTarget=enemy.IdleTargets[enemy.CurrentIdleTarget].GlobalPosition;
            enemy.QueueRedraw();
    }

}

using Godot;
using System;
using System.Diagnostics;

[GlobalClass]

public partial class DangerBehaviour : SteeringBehaviours
{
    float ColliderSize;
    [Export]float radius;
    //[Export] Collider2D ThisCollider;

    float[] dangersGizmo;

    [Export] bool ShowGizmo;
    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, EnemyOverworldController enemy)
    {
        for(int j = 0; j < Directions.directions.Count; j++)
        {
            var spaceState = enemy.GetWorld2D().DirectSpaceState;
            var query = PhysicsRayQueryParameters2D.Create(enemy.GlobalPosition, enemy.GlobalPosition+Directions.directions[j]*radius, enemy.Parent.CollisionMask);
            query.Exclude=new Godot.Collections.Array<Rid> {enemy.Parent.GetRid(),enemy.CurrentTarget.GetNode<CollisionObject2D>(".").GetRid()};
            var result = spaceState.IntersectRay(query);
            enemy.RaycastPos[j]=enemy.GlobalPosition+Directions.directions[j]*radius;
            if(result.Count>0){
            //Debug.WriteLine("Result: "+result);
                danger[j]=5;
                if(j<Directions.directions.Count-1 && j>0){
                    danger[j+1]=2;
                    danger[j-1]=2;
                }
                else if(j==Directions.directions.Count){
                    danger[j-1]=2;
                    danger[0]=2;
                }
                else if(j==0){
                    danger[j+1]=2;
                    danger[Directions.directions.Count-1]=2;
                }
            }
        }
            //enemy.QueueRedraw();
        /*for(int i = 0; i < enemy.Obstacles.Count;i++)
            {
                if (enemy.Obstacles[i] != null)
                {

                    Vector2 ObstacleDirection = enemy.Obstacles[i].ClosestPoint(transform.position) - (Vector2)transform.position;
                    float ObstacleDistance = ObstacleDirection.Length();

                    float weight;
                    if (ObstacleDistance <= ColliderSize)
                    {
                        weight= 1;
                    }
                    else
                    {
                        weight = /*(radius-ObstacleDistance)/radius/ObstacleDistance;
                    }
                    Vector2 NormalizedObstacleDirection = ObstacleDirection.normalized;

                }
            }*/
            dangersGizmo = danger;
            return (danger, interest);
    }

}

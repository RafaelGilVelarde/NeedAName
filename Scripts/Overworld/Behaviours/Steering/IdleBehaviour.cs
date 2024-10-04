using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;
[GlobalClass]
public partial class IdleBehaviour : SteeringBehaviours
{
    [Export]private float ReachThreshold;
    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, EnemyOverworldController enemy)
    {
            float Direction;

            Vector2 TargetDirection = enemy.CachedTargetPosition - (Vector2)enemy.GlobalPosition;
            
            Direction = TargetDirection.Length();
            if(Direction<ReachThreshold){
                FindOpponent(enemy);
                /*RandomNumberGenerator RNG=new RandomNumberGenerator();
                enemy.CurrentIdleTarget=RNG.RandiRange(0,enemy.IdleTargets.Count-1);*/
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
        bool InPolygon = false;
        Vector2 Random = Vector2.Zero;
        while(!InPolygon){
            Polygon2D Poly = enemy.detectArea.Polygon;
            RandomNumberGenerator RNG=new RandomNumberGenerator();
            RNG.Randomize();
            float RandX = RNG.RandfRange(enemy.RandMin.X,enemy.RandMax.X);
            float RandY = RNG.RandfRange(enemy.RandMin.Y,enemy.RandMax.Y);
            Random = new Vector2(RandX,RandY);
            InPolygon=Geometry2D.IsPointInPolygon(Random,Poly.Polygon);
        }
        enemy.CachedTargetPosition=Random+enemy.detectArea.GlobalPosition;
        /*
            var spaceState = enemy.GetWorld2D().DirectSpaceState;
            var query = PhysicsRayQueryParameters2D.Create(enemy.GlobalPosition, enemy.IdleTargets[enemy.CurrentIdleTarget].GlobalPosition, enemy.Parent.CollisionMask);
            query.Exclude=new Godot.Collections.Array<Rid> {enemy.Parent.GetRid()};
            query.CollideWithAreas=true;
            var result = spaceState.IntersectRay(query);
            if(result.Count>0){
                if(result["collider"].As<Node2D>()==enemy.IdleTargets[enemy.CurrentIdleTarget].GetChild<Node2D>(0)){
                   // Debug.WriteLine("This Position: "+enemy.GlobalPosition);
                    //Debug.WriteLine("Origin: "+enemy.Transform.Origin);
                    enemy.CachedTargetPosition=enemy.IdleTargets[enemy.CurrentIdleTarget].GlobalPosition;
                }

            }
            enemy.RaycastTarget=enemy.IdleTargets[enemy.CurrentIdleTarget].GlobalPosition;
            enemy.QueueRedraw();*/
    }


}

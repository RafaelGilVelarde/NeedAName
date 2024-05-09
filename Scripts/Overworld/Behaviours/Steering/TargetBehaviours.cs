using Godot;
using System;
using System.Diagnostics;
using System.Xml.XPath;

[GlobalClass]
public partial class TargetBehaviours : SteeringBehaviours
{
    [Export] private float ReachThreshold, InterestVariable;

    [Export]float[] interestGizmo;

    [Export] bool ShowGizmo;
    [Export] bool showResult;
    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, EnemyOverworldController enemy)
    {
            /*if (enemy.GlobalPosition.DistanceTo(CachedTargetPosition) < ReachThreshold)
            {
                //enemy.ReachedLastTarget = true;
                //enemy.CurrentTarget = null;
                return (danger, interest);
            }
            else
            {
                //enemy.ReachedLastTarget = false;
            }*/
            float Direction;
            Vector2 TargetDirection = enemy.CachedTargetPosition - (Vector2)enemy.GlobalPosition;
            
            Direction = TargetDirection.Length();
            if(Direction<ReachThreshold){
                enemy.state=MoveState.Idle;
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
                        if (showResult)
                        {
                            Debug.WriteLine("value "+j+": "+result);
                        }
                }

                interestGizmo = interest;
            }
            return (danger, interest);
    }
    public void FindOpponent(EnemyOverworldController enemy){
            var spaceState = enemy.GetWorld2D().DirectSpaceState;
            var query = PhysicsRayQueryParameters2D.Create(enemy.GlobalPosition, enemy.CurrentTarget.GlobalPosition, enemy.Parent.CollisionMask);
            query.Exclude=new Godot.Collections.Array<Rid> {enemy.Parent.GetRid()};
            var result = spaceState.IntersectRay(query);
            if(result.Count>0){
                if(result["collider"].As<Node2D>()==enemy.CurrentTarget && enemy.detectArea.Entered){
                    //Debug.WriteLine("result: "+result["position"]);
                    /*Debug.WriteLine("Position: "+enemy.CurrentTarget.GlobalPosition);
                    Debug.WriteLine("This Position: "+enemy.GlobalPosition);
                    Debug.WriteLine("Origin: "+enemy.Transform.Origin);*/
                    enemy.CachedTargetPosition=enemy.CurrentTarget.GlobalPosition;
                    enemy.state=MoveState.Seek;
                }
                /*else{
                    Debug.WriteLine("Cache: "+CachedTargetPosition);
                }*/
            }
            enemy.RaycastTarget=enemy.CurrentTarget.GlobalPosition;
            enemy.QueueRedraw();
    }

}

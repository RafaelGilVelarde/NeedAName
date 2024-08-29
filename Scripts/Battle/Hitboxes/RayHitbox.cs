using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public partial class RayHitbox : RayCast2D
{
    
	[Signal]
	public delegate void _HitEventHandler(BattleCharacter Target);
	[Signal]
	public delegate void _BlockEventHandler(BattleCharacter Target);

    public BattleCharacter Attacking;
    public override void _PhysicsProcess(double delta)
    {
        if(IsColliding()){
            OnCollisionEnter((CollisionObject2D)GetCollider());
            Enabled=false;
            
        }
    }
    void OnCollisionEnter(Node node) {
        if((node.IsInGroup("EnemyHurtbox")&&this.IsInGroup("PlayerHitbox"))||(node.IsInGroup("PlayerHurtbox")&&this.IsInGroup("EnemyHitbox"))){
			BattleCharacter Target=node.GetNode<BattleCharacter>("..");
		        for (int i=0;i<Attacking.Character.Equipment.Count;i++){
            		Attacking.Character.Equipment[i].Base.HitEnemyEffect(Attacking,Attacking.MoveUsed.Base);
        		}
			EmitSignal("_Hit",Target);
		}
		if((IsInGroup("EnemyHitbox")&&node.IsInGroup("PlayerBlockbox"))||(IsInGroup("PlayerHitbox")&&node.IsInGroup("EnemyBlockbox"))){
			BattleCharacter Target=node.GetNode<BattleCharacter>("..");
			EmitSignal("_Block",Target);
            EmitSignal("_Hit",Target);
        }
    }
    public void AddExceptions(Array<CollisionObject2D> Collisions){
        for(int i=0;i<Collisions.Count;i++){
            AddException(Collisions[i]);
        }
    }
}

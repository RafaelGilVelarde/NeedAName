using Godot;
using System;
using System.Diagnostics;

public partial class Hitbox : Area2D
{
	[Export]public bool CanBeBlocked;
	[Export]BattleCharacter Character;

	[Signal]
	public delegate void _HitEventHandler(BattleCharacter Target);
	[Signal]
	public delegate void _BlockEventHandler(BattleCharacter Target);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered+=OnTriggerEnter;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public virtual void OnTriggerEnter(Area2D node){
		if((node.IsInGroup("EnemyHurtbox")&&this.IsInGroup("PlayerHitbox"))||(node.IsInGroup("PlayerHurtbox")&&this.IsInGroup("EnemyHitbox"))){
		Debug.WriteLine(GetChild<CollisionShape2D>(0).Disabled);
			BattleCharacter Target=node.GetNode<BattleCharacter>("..");
			BattleCharacter Attacking=this.GetNode<BattleCharacter>("..");
		        for (int i=0;i<Attacking.Character.Equipment.Count;i++){
            		Attacking.Character.Equipment[i].Base.HitEnemyEffect(Attacking,Attacking.MoveUsed.Base);
        		}
			EmitSignal("_Hit",Target);
		}
		if((IsInGroup("EnemyHitbox")&&node.IsInGroup("PlayerBlockbox"))||(IsInGroup("PlayerHitbox")&&node.IsInGroup("EnemyBlockbox"))){
			Debug.WriteLine("DDDDDD");
			BattleCharacter Target=node.GetNode<BattleCharacter>("..");
			EmitSignal("_Block",Target);
        }
	} 
	public void getBlocked(){
		if(CanBeBlocked){
		}
	}

	
}

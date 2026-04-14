using Godot;
using System;
using System.Diagnostics;

public partial class Hitbox : Area2D
{
	[Export]public BattleCharacter Character;

	[Signal]
	public delegate void _HitEventHandler(BattleCharacter Target);
	[Signal]
	public delegate void _BlockEventHandler(BattleCharacter Target);
		[Signal]
	public delegate void _WPEventHandler(BattleCharacter Target);
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
		Debug.WriteLine(GetGroups()+ " Has Hit "+node.GetGroups());
		if((node.IsInGroup("EnemyHurtbox")&&this.IsInGroup("PlayerHitbox"))||(node.IsInGroup("PlayerHurtbox")&&this.IsInGroup("EnemyHitbox"))){
			BattleCharacter Target=node.GetNode<BattleCharacter>("..");
		        for (int i=0;i<Character.Character.Equipment.Count;i++){
            		Character.Character?.Equipment[i]?.HitEnemyEffect(Character,Character.MoveUsed.Base);
        		}
			EmitSignal("_Hit",Target);
		}
		if((IsInGroup("EnemyHitbox")&&node.IsInGroup("PlayerBlockbox"))||(IsInGroup("PlayerHitbox")&&node.IsInGroup("EnemyBlockbox"))){
			BattleCharacter Target=node.GetNode<BattleCharacter>("..");
			EmitSignal("_Block",Target);
        }
		if(node.IsInGroup("PlayerWPBox")&&this.IsInGroup("EnemyHitbox")){
			BattleCharacter Target=node.GetNode<BattleCharacter>("..");
		    EmitSignal("_WP",Target);
		}
	} 

	
}

using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class BossSpoutMove : MoveBase
{
	[Export] int Combo = 3;
    [Export] PackedScene Proyectile;
	[Export] float XOffset;
	[Export] string Tag;
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
        base.Effect(Users, Targets);
        
		SceneTreeTimer timer=Users[0].GetTree().CreateTimer(MoveTime,true,true);
   		//timer.TweenInterval(MoveTime);
		Users[0].changeCombo(Combo);
		Users[0].changeAction(BattleCharacter.ActionState.isAttacking);   
		
		Targets[0].Controllable=true;                      
		Targets[0].changeState(BattleCharacter.BattleState.Dodging);
        
		float Dir=(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X)/Mathf.Abs(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X);
		if(Dir==1||Dir==-1){
			Users[0].GetParent<Node2D>().Rotation=0;
			Users[0].GetParent<Node2D>().Scale=new Vector2(Dir,1);
			
			Targets[0].GetParent<Node2D>().Rotation=0;
			Targets[0].GetParent<Node2D>().Scale=new Vector2(-Dir,1);
		}		
		else{
			Dir=Users[0].GetParent<Node2D>().Scale.Y;
		}

		void onHit(BattleCharacter Target){
			Hit(Users[0],Target);
		}	

        Node2D proyectile=Proyectile.Instantiate<Node2D>();
		Hitbox Hitbox= (Hitbox)proyectile.GetChild(0);
		Hitbox.AddToGroup(Tag);
		Hitbox.Character=Users[0];
		Users[0]._Shoot+=shoot;
		Hitbox._Hit+=onHit;
		Hitbox._WP+=WPGraze;
		timer.Timeout+=End;

		void shoot(BattleCharacter User){
			CollisionShape2D Aux = Targets[0].Hurtbox.GetChild<CollisionShape2D>(0);
			proyectile.GlobalPosition = Aux.GlobalPosition-Users[0].Character.Base.BattleOffset+new Vector2(Dir*XOffset,((RectangleShape2D)Aux.Shape).Size.Y/2);
			User.GetTree().CurrentScene.AddChild(proyectile);
		}
		void End(){
			Users[0]._Shoot-=shoot;
		}

    }
}

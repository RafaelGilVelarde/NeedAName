using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class bridgeMoveTest : MoveBase
{
    DialogicCSharp Dialog;
    [Export] float Speed;
	[Export] Vector2 offset;

    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
        base.Effect(Users, Targets);
		Tween tween = Users[0].CreateTween();
        float Dir=(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X)/Mathf.Abs(Targets[0].GlobalPosition.X-Users[0].GlobalPosition.X);


        //Dialogic
        //Dialog.StartDialogue("MoveBattleTest",false,true);


		Targets[0].Controllable=true;                

		Vector2 TargetPosition = Targets[0].Hurtbox.GetChild<CollisionShape2D>(0).GlobalPosition-Users[0].BattleOffset;
        tween.TweenCallback(Callable.From(()=>Targets[0].changeState(BattleCharacter.BattleState.Dodging)));
		tween.TweenInterval(0.1);
        tween.TweenCallback(Callable.From(Dialogue));
		tween.TweenProperty(Users[0].GetParent(),"position",TargetPosition+offset*-Dir,1/Speed);
		tween.TweenCallback(Callable.From(()=>Users[0].changeAction(BattleCharacter.ActionState.isAttacking)));
		tween.TweenInterval(0.5);
		tween.TweenCallback(Callable.From(()=>Targets[0].changeState(BattleCharacter.BattleState.Defending)));
        tween.TweenInterval(1);
        
        tween.TweenCallback(Callable.From(()=>Attack2()));

        void Dialogue(){
            BattleManager.instance.Scene.StartDialogue("MoveBattleTest",false,true,TimelineType.Move);
        }
        void Attack2(){
            Users[0].changeCombo(2);
            Users[0].changeState(BattleCharacter.BattleState.Attacking);
            Users[0].changeAction(BattleCharacter.ActionState.isAttacking);
            Users[0].StatMultiplier[0]+=1.5f;
        }
        //Tween tween = Users[0].CreateTween();
        
    }
}

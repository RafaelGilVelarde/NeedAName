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


        Dialog=DialogicCSharp.instance;
        //Dialogic
        Dialog.StartDialogue("MoveBattleTest",false,true);
        Dialog.CallDeferred("AutoAdvance",true,false); 


		Targets[0].Controllable=true;                


        tween.TweenCallback(Callable.From(()=>Targets[0].changeState(BattleCharacter.BattleState.Dodging)));
		tween.TweenProperty(Users[0].GetParent(),"position",Targets[0].GetParent<Node2D>().Position+offset*-Dir,1/Speed);
		tween.TweenCallback(Callable.From(()=>Users[0].changeAction(BattleCharacter.ActionState.isAttacking)));
		tween.TweenInterval(0.5);
		tween.TweenCallback(Callable.From(()=>Targets[0].changeState(BattleCharacter.BattleState.Defending)));
        tween.TweenInterval(1);
        
        tween.TweenCallback(Callable.From(()=>Attack2()));

        void Attack2(){
            Users[0].Combo=2;
            Users[0].changeState(BattleCharacter.BattleState.Attacking);
            Users[0].changeAction(BattleCharacter.ActionState.isAttacking);
            Users[0].AddAtkMultiplier(1.5f,1);
            Debug.WriteLine("Timer: "+Users[0].TimerAtk);
        }
        //Tween tween = Users[0].CreateTween();
        
    }
}

using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class SuckMove : MoveBase
{
    [Export]float MinVel, MaxVel, SuckTimerMax, Speed;
    [Export] int Combo, Combo2;
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
        base.Effect(Users, Targets);
        Vector2 Origin = Targets[0].GlobalPosition;
        Vector2 Direction = Users[0].ShootNode.GlobalPosition-Origin;
        float VelocityMult = 0;
        Targets[0].Moving = true;
        CharacterBody2D Target = Targets[0].Overworld.Parent;
        bool Sucked = false, CanShoot = false, SuccesfulShoot = false;

        Users[0]._DoAction+=Spam;
        Users[0].Hurtbox.AreaEntered+=ShootStart;
        Users[0]._Shoot+=Shoot;
        SceneTreeTimer SuckTimer = Users[0].GetTree().CreateTimer(SuckTimerMax,true,true);
        SuckTimer.Timeout+=CheckSuck;

        Users[0].changeState(BattleCharacter.BattleState.Attacking);

        void Spam(){
            VelocityMult+=0.2f;
            float ClampVelocity = Mathf.Clamp(VelocityMult,MinVel,MaxVel);
            Target.Velocity = Direction*VelocityMult;
        }
        void CheckSuck(){
            if(!Sucked){
                MainTimer.TimeLeft = 0;
                Users[0]._DoAction-=Spam;
                End();
            }
        }
        void ShootStart(Area2D Area){
            Area2D HurtBox = Users[0].Hurtbox;
            if((HurtBox.IsInGroup("PlayerHurtbox") && Area.IsInGroup("EnemyHurtbox")) || (HurtBox.IsInGroup("EnemyHurtbox") && Area.IsInGroup("PlayerHurtbox"))){
                Users[0]._DoAction-=Spam;
                Users[0]._DoAction+=ShootInput;
                Targets[0].Moving = false;
                Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",true);
                Users[0].AnimatorTree.Set("parameters/ActionState/0/0/"+Combo+"/conditions/MoveEnded",false);
                Users[0].RemoveAttack();

                SceneTreeTimer WindupTimer = Users[0].GetTree().CreateTimer(0.2,true,true);
                WindupTimer.Timeout+=()=>{CanShoot = true;
                    Users[0].Character.ShowTextLabel($"{Users[0].Character.Key}",Users[0].Character.Base.TextEffectColor);
                    SceneTreeTimer ShootTimer = Users[0].GetTree().CreateTimer(0.5,true,true);
                    ShootTimer.Timeout+=()=>{
                        if(CanShoot){
                            CanShoot = false;
                            Users[0].changeCombo(2);
                            Users[0].changeAction(BattleCharacter.ActionState.isAttacking);
                        }
                        Users[0]._DoAction-=ShootInput;
                    };
                };
            }
            HurtBox.AreaEntered-=ShootStart;
        }
        void ShootInput(){
            if(CanShoot){
                CanShoot = false;
                SuccesfulShoot = true;
                Users[0].changeCombo(2);
                Users[0].changeAction(BattleCharacter.ActionState.isAttacking);
            }
        }
        void Shoot(BattleCharacter UserCharacter){
            Tween tween = Target.CreateTween();
            if(SuccesfulShoot){
                tween.TweenProperty(Target,"position",Origin,1/Speed);
                Users[0].Character.ChangeWP(WP);
            }
            else{
                tween.TweenProperty(Target,"position",Origin,1/(Speed*2));
            }
            tween.Finished+=()=>{
                Hit(UserCharacter,Targets[0]);
                End();
                tween.Kill();
            };
            UserCharacter._Shoot-=Shoot;
        }

        void End(){
            Targets[0].Velocity = Vector2.Zero;
            Users[0].Hurtbox.AreaEntered-=ShootStart;
        }
    }

}

using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class SuckMove : MoveBase
{
    [Export]float MinVel, MaxVel, SuckTimerMax, Speed;
    [Export] int Combo, Combo2;
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
        base.Effect(Users, Targets);
        Vector2 TargetHurtboxPosition = Targets[0].Hurtbox.Position;
        Vector2 Origin = Targets[0].GlobalPosition;
        Vector2 Direction = Vector2.Zero;
        Targets[0].Hurtbox.GlobalPosition = Origin;

        float VelocityMult = 0;
        Targets[0].Moving = true;
        CharacterBody2D Target = Targets[0].Overworld.Parent;
        Vector2 Scale = Target.Scale;
        Target.Velocity = Vector2.Zero;
        bool Sucked = false, CanShoot = false, SuccesfulShoot = false;

        Tween Rotate = Users[0].CreateTween();
        float Angle = Users[0].ShootNode.GlobalPosition.AngleToPoint(Targets[0].Hurtbox.GlobalPosition);
        Rotate.TweenProperty(Users[0].GetParent(),"global_rotation", Angle,0.1f);
        Rotate.Finished += () =>
        {
            Users[0]._DoAction += Spam;
            Users[0].Hurtbox.AreaEntered += ShootStart;
            SceneTreeTimer SuckTimer = Users[0].GetTree().CreateTimer(SuckTimerMax, true, true);
            SuckTimer.Timeout += CheckSuck;

            Users[0].changeState(BattleCharacter.BattleState.Attacking);
            Rotate.Kill();
        };


        void Spam(){
            if (VelocityMult == 0)
            {
                Users[0].RemoveAttack();
                SceneTreeTimer AuxTimer = Users[0].GetTree().CreateTimer(0.3, true, true);
                AuxTimer.Timeout += () =>
                {
                    Direction = Users[0].ShootNode.GlobalPosition - Origin;
                    Debug.WriteLine("Direction: " + Direction);
                    Debug.WriteLine("Shootnode Position: " + Users[0].ShootNode.Position);
                    IncreaseVelocity();
                };
            }
            else
            {
                IncreaseVelocity();
            }
        }
        void IncreaseVelocity()
        {
            VelocityMult +=0.2f;
            float ClampVelocity = Mathf.Clamp(VelocityMult,MinVel,MaxVel);
            Target.Velocity = Direction*VelocityMult;
            Target.Scale *= 0.8f;              
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
            if ((HurtBox.IsInGroup("PlayerHurtbox") && Area.IsInGroup("EnemyHurtbox")) || (HurtBox.IsInGroup("EnemyHurtbox") && Area.IsInGroup("PlayerHurtbox")))
            {
                Sucked = true;
                Users[0]._Shoot+=Shoot;
                Users[0]._DoAction -= Spam;
                Users[0]._DoAction += ShootInput;
                Targets[0].Moving = false;
                    CanShoot = true;
                    Users[0].Character.ShowTextLabel($"{Users[0].Character.Key}", Users[0].Character.Base.TextEffectColor);
                    SceneTreeTimer ShootTimer = Users[0].GetTree().CreateTimer(0.3, true, true);
                    ShootTimer.Timeout += () =>
                    {
                        Targets[0].Visible = false;
                        Users[0]._DoAction -= ShootInput;
                        if (CanShoot)
                        {
                            Debug.WriteLine("Changing Animation");
                            Users[0].AnimatorTree.Set("parameters/ActionState/0/0/" + Combo + "/conditions/MoveEnded", true);
                            CanShoot = false;
                        }
                    };
            HurtBox.AreaEntered-=ShootStart;
            }
        }
        void ShootInput(){
            if(CanShoot){
                CanShoot = false;
                SuccesfulShoot = true;
                Users[0].AnimatorTree.Set("parameters/ActionState/0/0/" + Combo + "/conditions/MoveEnded", true);
            }
        }
        void Shoot(BattleCharacter UserCharacter){
            Targets[0].Visible = true;
            Tween tween = Target.CreateTween();
            if(SuccesfulShoot){
                tween.TweenProperty(Target,"position",Origin,1/(Speed*2));
                Users[0].Character.ChangeWP(WP);
            }
            else{
                tween.TweenProperty(Target,"position",Origin,1/Speed);
            }
            tween.Finished+=()=>{
                Hit(UserCharacter,Targets[0]);
                End();
                tween.Kill();
            };
            UserCharacter._Shoot-=Shoot;
        }

        void End()
        {
            Users[0].AnimatorTree.Set("parameters/ActionState/0/0/" + Combo + "/conditions/MoveEnded", false);
            Target.Velocity = Vector2.Zero;
            Targets[0].Hurtbox.Position = TargetHurtboxPosition;
            if (!Sucked)
            {
                Users[0].Hurtbox.AreaEntered -= ShootStart;
            }
            Tween tween = Target.CreateTween();
            tween.TweenProperty(Target, "scale", Scale, 0.2f).SetEase(Tween.EaseType.OutIn);
            MainTimer.TimeLeft = 0.2f;
        }
    }

}

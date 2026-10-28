using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

public partial class ThrowProjectile : Node2D
{
    [Export] Array<PackedScene> Projectiles;
    [Export] int ProjectileIndex;
    [Export] float Middle, Height;
    [Export] Array<Vector2> OffsetBounds;
    [Export] Node2D ShootNode, User;
    [Export] public Node2D Target;
    [Export] bool CanShoot;

    public override void _Ready()
    {
        base._Ready();
        Target = GameManager.Instance.Characters[0];
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (CanShoot)
        {
        Throw();
            
        }
    }
    public void Throw()
    {
        CanShoot = false;
        RandomNumberGenerator RNGX=new RandomNumberGenerator();
        RandomNumberGenerator RNGY=new RandomNumberGenerator();
        Vector2 Offset = new Vector2(RNGX.RandfRange(OffsetBounds[0].X,OffsetBounds[1].X),RNGY.RandfRange(OffsetBounds[0].Y,OffsetBounds[1].Y));


        BezierProyectile Projectile = Projectiles[ProjectileIndex].Instantiate<BezierProyectile>();
        Projectile.GlobalPosition = ShootNode.GlobalPosition;
        Projectile.PosStart = ShootNode.GlobalPosition;
        Projectile.PosEnd = Target.GlobalPosition + Offset;
        Projectile.PosMiddle = (Projectile.PosStart+Projectile.PosEnd)/2;
        float MiddleX =Projectile.PosStart.X+((Projectile.PosEnd.X-Projectile.PosStart.X)/Middle);
		Projectile.PosMiddle=new Vector2(MiddleX,User.GlobalPosition.Y-Height);

        Projectile.GlobalScale=User.GlobalScale;
		Projectile.GlobalRotation=User.GlobalRotation;
		Projectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Hit",false);
		Projectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Start",true);	
        Projectile.Ended = true;


		if(Projectile.GetParent()==null){
			User.GetTree().CurrentScene.AddChild(Projectile);
		}
        Projectile.TreeExited+=()=>{CanShoot = true;};
        Projectile._EndPath+=Projectile.Hit;
        Projectile.GetChild<OverworldHitbox>(0)._Hit +=Projectile.HitWrapper;
		Projectile.Start();
    }



    
}


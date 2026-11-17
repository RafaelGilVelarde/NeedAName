using Godot;
using Godot.Collections;
using System;

public enum VisualType{
    None,
    Animation,
    Color,
    Sprite
}
public partial class Interact : CollisionShape2D
{
        [Export] protected Vector2 FacingDirection;
    [Export] protected AnimationPlayer Animator;
    [Export] public AnimationTree AnimatorTree;
    [Export] protected VisualType visualType;
    [Export] protected Sprite2D MainSprite;
    [Export] protected Array<Sprite2D> Sprites;
    [Export] protected Array<Color> Colors;

      [Export] protected bool FacingRight = true, Cutscene, Turn;
    [Export] public bool SpokenTo;

    public override void _Ready()
    {
        base._Ready();
                if(GetChildCount()>0){
            if(GetChild(0)?.GetChildCount()>0){
                Node Aux=GetChild(0).GetChild(0);
                if(Aux.GetType()==typeof(AnimationPlayer)){
                    Animator = (AnimationPlayer)Aux;
                }
            }
        }
        if(Animator?.GetChildCount()>0){
            Node Aux=Animator.GetChild(0);
            if(Aux.GetType()==typeof(AnimationTree)){
                AnimatorTree = (AnimationTree)Aux;

            }
        }
    }
    public virtual void interact(OverworldController Player)
    {
        if (Turn)
        {
            FacingDirection = Player.FacingDirection * -1;
            if (FacingDirection.X != 0)
            {
                Flip();
            }
            AnimatorTree?.Set("parameters/Idle/blend_position", new Vector2(FacingDirection.X, -FacingDirection.Y));
        }
    }
    protected virtual void Flip()
    {
        if (FacingDirection.X / Mathf.Abs(FacingDirection.X) > 0 != FacingRight)
        {
            Scale = new Vector2(Scale.X * -1, Scale.Y);
            FacingRight = !FacingRight;
        }
    }

}

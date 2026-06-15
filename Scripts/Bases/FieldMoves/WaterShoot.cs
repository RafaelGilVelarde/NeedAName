using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

[GlobalClass]
public partial class WaterShoot : FieldMoveBase
{
    [Export] PackedScene Bubble;
    [Export] int Distance;
    [Export] float Height;
    public override void Effect(OverworldController Overworld)
    {
        base.Effect(Overworld);
        Debug.WriteLine("Shooting");
        BezierProyectile proyectile = Bubble.Instantiate<BezierProyectile>();
        Vector2 Origin = Overworld.Parent.GlobalPosition;
        Vector2I TargetTile = (Vector2I)(Overworld.Coords + new Vector2I(Distance,Distance) * Overworld.FacingDirection);
        Vector2 Target = Overworld.CurrentMapLayer.MapToLocal(TargetTile);
        
        Debug.WriteLine("Origin Tile: "+Overworld.Coords);
        Debug.WriteLine("Tile: "+TargetTile);
        Debug.WriteLine("Origin: "+Origin);
        Debug.WriteLine("Target: "+Target);

        proyectile.Ended = true;
        proyectile.PosStart = Origin;
        proyectile.PosEnd = Target;
        Vector2 AuxMiddle = (Origin+Target)/2;
        if(AuxMiddle.X == 0)
        {
            proyectile.PosMiddle = new Vector2(Origin.X+Height,AuxMiddle.Y);            
        }
        else
        {
            proyectile.PosMiddle = new Vector2(AuxMiddle.X,Origin.Y+Height);            
        }
        proyectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Hit",false);
		proyectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Start",true);			
		if(proyectile.GetParent()==null){
			Overworld.GetTree().CurrentScene.AddChild(proyectile);
		}
        proyectile._EndPath+=EndProyectile;
		proyectile.Start();


        void EndProyectile()
        {
            proyectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Hit",true);
		    proyectile.GetChild<AnimationPlayer>(2).GetChild<AnimationTree>(0).Set("parameters/conditions/Start",false);		
            proyectile.Acting = false;
        }
    }
}

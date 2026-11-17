using Godot;
using Godot.Collections;
using System;

public partial class MovingObject : CharacterBody2D
{
    
    [Export] TileCheck Tile;
    [Export] Vector2 Axis;
    [Export] float Speed;
    public override void _Process(double delta)
    {
        base._Process(delta);
        Tile.CheckTile();
        switch (Tile.tileTypes)
        {
            case TileTypes.Lava:
                if (Tile.tileTypes != Tile.PreviousTileType)
                    {
                        GlobalPosition = Tile.SceneCoords;
                        TileData TileAux = Tile.DataMap.Map[ZIndex%Tile.DataMap.Map.Count].GetCellTileData(Tile.Coords);
                        Axis = (Vector2)TileAux.GetCustomData("Direction");
                    }
                if (Tile.DataMap != null)
                    {
                        TileData TileAux = Tile.DataMap.Map[ZIndex%Tile.DataMap.Map.Count].GetCellTileData(Tile.Coords);
                        if (TileAux != null)
                            {
                                Axis = Axis.MoveToward((Vector2)TileAux.GetCustomData("Direction"), (float)(delta*60/16));															
                            }
                    }
            break;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		Move(Speed * Axis);
        MoveAndSlide();
    }

    void Move(Vector2 Accel)
    {
        Velocity = Accel;
    }

}

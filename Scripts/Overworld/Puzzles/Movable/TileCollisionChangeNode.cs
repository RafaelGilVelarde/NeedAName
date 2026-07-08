using Godot;
using System;
using System.Diagnostics;

public partial class TileCollisionChangeNode : Node2D
{
    [Export] public Vector2I TilesetChangeTo;
    //[Export] public Vector2I CurrentTile;
    [Export] public CollisionShape2D Collider;
    TileMapLayer CollisionMap;

    Vector2I CurrentPosition;
    public override void _Ready()
    {
        base._Ready();
        CollisionMap = CollisionTileMap.Instance.Map[0];
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        /*Vector2I Position = CollisionMap.LocalToMap(GlobalPosition);
        TileData Data = CollisionMap.GetCellTileData(Position);
        
        if(Position != CurrentPosition)
        {
            TileData PreviousData = CollisionMap.GetCellTileData(CurrentPosition);
            if(PreviousData != null)
            {
                if((bool)PreviousData.GetCustomData("Changeable") == true)
                {
                    Collider.Disabled = false;
                    CollisionMap.SetCell(CurrentPosition,0,CurrentTile,1);
                    Debug.WriteLine("ChangingBack");
                }  
            }
            CurrentTile = CollisionMap.GetCellAtlasCoords(Position);

            if (Data != null)
            {
                if((bool)Data.GetCustomData("Changeable") == true)
                {
                    Debug.WriteLine("Changing");
                    Collider.Disabled = true;
                    CollisionMap.SetCell(Position,0,TilesetChangeTo,1);
                }
            }
            
            CurrentPosition = Position;
        }*/
    }
    public void Disable()
    {
        Vector2I Position = CollisionMap.LocalToMap(GlobalPosition);
        
        if(Position != CurrentPosition)
        {
            TileData PreviousData = CollisionMap.GetCellTileData(CurrentPosition);
            if(PreviousData != null)
            {
                if((bool)PreviousData.GetCustomData("Changeable") == true)
                {
                    Collider.Disabled = false;
                    Vector2I OriginalTile = CollisionTileMap.Instance.OriginalCells[CollisionTileMap.Instance.UsedCells.IndexOf(CurrentPosition)];
                    CollisionMap.SetCell(CurrentPosition,0,OriginalTile,1);
                    Debug.WriteLine("ChangingBack");
                }  
            }

                //CurrentTile = CollisionMap.GetCellAtlasCoords(Position);                
        }
    }
    public void Enable()
    {
        Vector2I Position = CollisionMap.LocalToMap(GlobalPosition);
        TileData Data = CollisionMap.GetCellTileData(Position);
        if(Position != CurrentPosition)
        {
            if (Data != null)
                {
                    if((bool)Data.GetCustomData("Changeable") == true)
                    {
                        Debug.WriteLine("Changing");
                        Collider.Disabled = true;
                        CollisionMap.SetCell(Position,0,TilesetChangeTo,1);
                    }
                }
                
            CurrentPosition = Position;            
        }
    }

}

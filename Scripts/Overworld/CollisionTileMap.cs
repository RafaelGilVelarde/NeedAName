using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class CollisionTileMap: Node2D
{
    [Export]public Array<TileMapLayer> Map;
    public static CollisionTileMap Instance;

    public Array<Vector2I> OriginalCells = new Array<Vector2I>();
    public Array<Vector2I> UsedCells = new Array<Vector2I>();
    public override void _Ready()
    {
        Instance = this;
        UsedCells = Map[0].GetUsedCells();
        for(int i = 0; i < UsedCells.Count; i++)
        {
            Debug.WriteLine("Cell: "+UsedCells[i]);
            OriginalCells.Add(Map[0].GetCellAtlasCoords(UsedCells[i]));
            /*int CurrentY = 0;
            if(UsedCells[i]!=new Vector2I(-1, -1))
            {
                if (UsedCells[i].Y >= CurrentY)
                {
                    CurrentY++;
                    OriginalCells.Add(new Array<Vector2I>());
                }
                OriginalCells[CurrentY].Add(Map[0].GetCellAtlasCoords(UsedCells[i]));
            }
            else
            {
                OriginalCells[CurrentY].Add(new Vector2I(-1,-1));
            }*/
        }
    }
}

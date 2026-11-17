using Godot;
using System;

public enum TileTypes{
	Normal,
	Water,
	Lava
}
public partial class TileCheck : Node2D
{
    [Export] Node2D BaseNode;
    [Export] public Vector2 SceneCoords;
    [Export] public TileTypes tileTypes, PreviousTileType;
    [Export] public Vector2I PrevCoords, Coords;
	[Export] public TileMapLayer CurrentMapLayer;
	[Export] public DataTileMap DataMap;


    public void CheckTile()
    {
        	TileData Data = null;
			SceneCoords = new Vector2();
			if (DataMap != null)
			{
				CurrentMapLayer = DataMap.Map[BaseNode.ZIndex%DataMap.Map.Count];
                Coords = CurrentMapLayer.LocalToMap(BaseNode.GlobalPosition);

				Data = CurrentMapLayer.GetCellTileData(Coords);
				SceneCoords = CurrentMapLayer.MapToLocal(Coords);
			}
			if(Data!=null){
				PreviousTileType = tileTypes;
				tileTypes = (TileTypes)(int)Data.GetCustomData("TileType");
			}
    }
}

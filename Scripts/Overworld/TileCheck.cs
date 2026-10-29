using Godot;
using System;

public enum TileTypes{
	Normal,
	Water
}
public partial class TileCheck : Node2D
{
    [Export] Node2D BaseNode;
    [Export] public Vector2 FacingDirection, SceneCoords;
    [Export] public TileTypes tileTypes, PreviousTileType;
    [Export] protected Vector2 TileOffset;
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
				//PrevCoords = Coords;
				/*Vector2I BaseCoords = CurrentMapLayer.LocalToMap(BaseNode.GlobalPosition);
				Vector2I CoordAux = BaseCoords - PrevCoords;*/
                //Coords = CurrentMapLayer.LocalToMap(BaseNode.GlobalPosition - (CoordAux * TileOffset));
                /*
				if(CoordAux.X!= FacingDirection.X || CoordAux.Y!= FacingDirection.Y)
				{
					Coords = BaseCoords;
				}
				else
				{
					Coords = CurrentMapLayer.LocalToMap(BaseNode.GlobalPosition - (FacingDirection * TileOffset));
				}*/

				Data = CurrentMapLayer.GetCellTileData(Coords);
				SceneCoords = CurrentMapLayer.MapToLocal(Coords);
			}
			if(Data!=null){
				PreviousTileType = tileTypes;
				tileTypes = (TileTypes)(int)Data.GetCustomData("TileType");
			}
    }
}

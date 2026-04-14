using Godot;
using Godot.Collections;
using System;

public partial class DataTileMap : Node2D
{
    public Array<TileMapLayer> Map;
    public override void _Ready()
    {
        GameManager.Instance.SetDataTileMap(this);
    }
}

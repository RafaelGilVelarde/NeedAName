using Godot;
using System;

public partial class DataTileMap : TileMap
{
    public override void _Ready()
    {
        GameManager.Instance.SetDataTileMap(this);
    }
}

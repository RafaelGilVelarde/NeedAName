using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;

public partial class DataTileMap : Node2D
{
    [Export]public Array<TileMapLayer> Map;
    public override void _Ready()
    {
        GameManager.Instance.SetDataTileMap(this);
    }
}

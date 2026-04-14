using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class WaterCurrentPuzzle : PuzzleCheck
{
    [Export] Array<WaterCurrents> CurrentArray;
    [Export] DataTileMap DataTileMaps;
    Array<TileMapLayer> DataTileMap;
    Array<Array<Vector2I>> Cells = new Array<Array<Vector2I>>();
    Array<Array<Array<Vector2I>>> AtlasCoords = new Array<Array<Array<Vector2I>>>();

    
    public override void _Ready()
    {
        DataTileMap = DataTileMaps.Map;
        for(int i = 0;i<CurrentArray.Count;i++){ //4
            AtlasCoords.Add(new Array<Array<Vector2I>>());
            Cells.Add(new Array<Vector2I>());
            for (int j = 0;j<CurrentArray[i].Currents.Map.Count;j++){ //2
                AtlasCoords[i].Add(new Array<Vector2I>());
                Array<Vector2I> Aux = CurrentArray[i].Currents.Map[j].GetUsedCells();
                for(int k = 0; k < Aux.Count;k++){ //X
                    Cells[i].Add(Aux[k]);
                    AtlasCoords[i][j].Add(DataTileMap[j+1].GetCellAtlasCoords(Aux[k]));
                }
            }
        }
        base._Ready();
    }
    void ChangeMap(int CurrentIndex){
        DataTileMap CurrentCurrents = CurrentArray[CurrentIndex].Currents;
        //int EventIndex = CurrentArray[CurrentIndex].CurrentEventIndex;
        Flags flags = GameManager.Instance.Data.Flags;
        if(!flags.PuzzleFlags[FlagIndex]){
            CurrentCurrents.Map[0].Show();
            CurrentCurrents.Map[1].Hide();


            int CellPointer;
            Array<Vector2I> Aux = Cells[CurrentIndex];
            CellPointer = 0;
            for(int i = 0;i < AtlasCoords[CurrentIndex][0].Count;i++){
                DataTileMap[0].SetCell(Aux[i + CellPointer],1,AtlasCoords[CurrentIndex][0][i]);
            }   
        }
        else{
            CurrentCurrents.Map[1].Show();
            CurrentCurrents.Map[0].Hide();     

            int CellPointer;
            Array<Vector2I> Aux = Cells[CurrentIndex];
            CellPointer = Aux.Count - Aux.Count/2;
            for(int i = 0;i < AtlasCoords[CurrentIndex][1].Count;i++){
                DataTileMap[0].SetCell(Aux[i + CellPointer],1,AtlasCoords[CurrentIndex][1][i]);
            }   
        }
    }
    public override void Check(int Index, bool Changed)
    {
        if(FlagIndex == Index){
            ActivateEffect();
        }
    }
    public override void ActivateEffect()
    {
        base.ActivateEffect();
        for(int i = 0; i < CurrentArray.Count; i++){
            ChangeMap(i);
        }
    }
}

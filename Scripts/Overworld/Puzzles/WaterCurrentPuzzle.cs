using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class WaterCurrentPuzzle : PuzzleCheck
{
    [Export] Array<WaterCurrents> CurrentArray;
    [Export] TileMap DataTileMap;

    Array<Array<Vector2I>> Cells = new Array<Array<Vector2I>>();
    Array<Array<Array<Vector2I>>> AtlasCoords = new Array<Array<Array<Vector2I>>>();

    
    public override void _Ready()
    {
        //Array<Vector2I> Cells = DataTileMap.GetUsedCells(4);

        for(int i = 0;i<CurrentArray.Count;i++){ //4
            AtlasCoords.Add(new Array<Array<Vector2I>>());
            Cells.Add(new Array<Vector2I>());
            for (int j = 0;j<CurrentArray[i].Currents.Count;j++){ //2
                AtlasCoords[i].Add(new Array<Vector2I>());
                Array<Vector2I> Aux = CurrentArray[i].Currents[j].GetUsedCells(4);
                for(int k = 0; k < Aux.Count;k++){ //X
                    Cells[i].Add(Aux[k]);
                    AtlasCoords[i][j].Add(DataTileMap.GetCellAtlasCoords(j+1,Aux[k]));
                }
            }
        }
        base._Ready();
    }
    void ChangeMap(int CurrentIndex){
        Array<TileMap> CurrentCurrents = CurrentArray[CurrentIndex].Currents;
        //int EventIndex = CurrentArray[CurrentIndex].CurrentEventIndex;
        Flags flags = GameManager.Instance.Data.Flags;
        if(!flags.PuzzleFlags[FlagIndex]){
            CurrentCurrents[0].Show();
            CurrentCurrents[1].Hide();
            /*
            Array<Vector2I> Cells = new Array<Vector2I>();
            Array<Vector2I> AtlasCoords = new Array<Vector2I>();
            for(int i = 0;i < CurrentCurrents.Count;i++){
                Array<Vector2I> Aux = CurrentCurrents[i].GetUsedCells(4);
                for(int j = 0; j < Aux.Count;j++){
                    Cells.Add(Aux[j]);
                    AtlasCoords.Add(DataTileMap.GetCellAtlasCoords(1,Cells[Cells.Count-1]));
                }
            }*/

            int CellPointer;
            Array<Vector2I> Aux = Cells[CurrentIndex];
            CellPointer = 0;
            for(int i = 0;i < AtlasCoords[CurrentIndex][0].Count;i++){
                DataTileMap.SetCell(0,Aux[i + CellPointer],1,AtlasCoords[CurrentIndex][0][i]);
            }   
        }
        else{
            CurrentCurrents[1].Show();
            CurrentCurrents[0].Hide();     
            /*Array<Vector2I> Cells = new Array<Vector2I>();
            Array<Vector2I> AtlasCoords = new Array<Vector2I>();
            for(int i = 0;i < CurrentCurrents.Count;i++){
                Array<Vector2I> Aux = CurrentCurrents[i].GetUsedCells(4);
                for(int j = 0; j < Aux.Count;j++){
                    Cells.Add(Aux[j]);
                    AtlasCoords.Add(DataTileMap.GetCellAtlasCoords(2,Cells[Cells.Count-1]));
                }
            }

            for(int i = 0;i < Cells.Count;i++){
                DataTileMap.SetCell(0,Cells[i],-1,AtlasCoords[i]);
            }    */
            int CellPointer;
            Array<Vector2I> Aux = Cells[CurrentIndex];
            CellPointer = Aux.Count - Aux.Count/2;
            for(int i = 0;i < AtlasCoords[CurrentIndex][1].Count;i++){
                DataTileMap.SetCell(0,Aux[i + CellPointer],1,AtlasCoords[CurrentIndex][1][i]);
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

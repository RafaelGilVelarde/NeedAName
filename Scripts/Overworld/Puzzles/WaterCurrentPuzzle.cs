using Godot;
using Godot.Collections;
using System;

public partial class WaterCurrentPuzzle : Scene
{
    [Export] Array<WaterCurrents> CurrentArray;
    [Export] TileMap DataTileMap;

    Array<Array<Vector2I>> Cells = new Array<Array<Vector2I>>();
    Array<Array<Array<Vector2I>>> AtlasCoords = new Array<Array<Array<Vector2I>>>();

    
    public override void _Ready()
    {
        base._Ready();
        //Array<Vector2I> Cells = DataTileMap.GetUsedCells(4);

        for(int i = 0;i<CurrentArray.Count;i++){
            AtlasCoords.Add(new Array<Array<Vector2I>>());
            Cells.Add(new Array<Vector2I>());
            for (int j = 0;j<CurrentArray[i].Currents.Count;j++){
                AtlasCoords[i].Add(new Array<Vector2I>());
                Array<Vector2I> Aux = CurrentArray[i].Currents[j].GetUsedCells(4);
                for(int k = 0; k < Aux.Count;k++){
                    Cells[i].Add(Aux[k]);
                    AtlasCoords[i][j].Add(DataTileMap.GetCellAtlasCoords(j+1,Aux[k]));
                }
            }
        }
    }
    void ChangeMap(int CurrentIndex){
        Array<TileMap> CurrentCurrents = CurrentArray[CurrentIndex].Currents;
        int EventIndex = CurrentArray[CurrentIndex].CurrentEventIndex;
        Flags flags = GameManager.Instance.Data.Flags;
        if(flags.PuzzleFlags[EventIndex]){
            /*CurrentCurrents[0].Hide();
            CurrentCurrents[1].Show();
            Array<Vector2I> Cells = new Array<Vector2I>();
            Array<Vector2I> AtlasCoords = new Array<Vector2I>();
            for(int i = 0;i < CurrentCurrents.Count;i++){
                Array<Vector2I> Aux = CurrentCurrents[i].GetUsedCells(4);
                for(int j = 0; j < Aux.Count;j++){
                    Cells.Add(Aux[j]);
                    AtlasCoords.Add(DataTileMap.GetCellAtlasCoords(1,Cells[Cells.Count-1]));
                }
            }*/

            for(int i = 0;i < Cells[CurrentIndex].Count;i++){
                Array<Vector2I> Aux = Cells[CurrentIndex];
                DataTileMap.SetCell(0,Aux[i],-1,AtlasCoords[CurrentIndex][0][i]);
            }
        }
        else{
            CurrentCurrents[1].Hide();
            CurrentCurrents[0].Show();     
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
            for(int i = 0;i < Cells[CurrentIndex].Count;i++){
                Array<Vector2I> Aux = Cells[CurrentIndex];
                DataTileMap.SetCell(0,Aux[i],-1,AtlasCoords[CurrentIndex][1][i]);
            }   
        }
    }
}

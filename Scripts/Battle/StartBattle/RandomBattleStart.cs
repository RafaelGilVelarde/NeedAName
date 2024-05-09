using Godot;
using Godot.Collections;
using System;

public partial class RandomBattleStart : Node
{
    [Export] BattleScene scene;
    [Export] Array<BattleCharacter>Enemy;
    [Export] Array<Vector2>PartyPos,EnemyPos;
    [Export] Vector2 CenterView;
    public override void _Process(double delta)
    {

            }
    /*public void StartBattle(){
        Array<OverworldController> Characters=GameManager.Instance.Characters;
        Array<BattleCharacter> Party=new Array<BattleCharacter>();
        for(int i=0;i<Characters.Count;i++){
            Party.Add(Characters[i].BattleCharacter);
        }
        BattleManager.instance.ProcessMode=ProcessModeEnum.Inherit;
        BattleManager.instance.StartBattle(scene,Party,Enemy,PartyPos,EnemyPos, CenterView,new Vector2(0,0));
        GameManager.Instance.BattleStart();
    }*/
}

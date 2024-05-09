using Godot;
using Godot.Collections;
using System;
[GlobalClass]
public partial class BattleScene : Resource
{
    DialogicCSharp Dialog;
    BattleManager Battle;
    Callable endDialogue;
    bool DialogueFlag;
    public virtual void StartBattleEffect(){
        Dialog=DialogicCSharp.instance;
        Battle=BattleManager.instance;
        endDialogue=new Callable(this,MethodName.EndDialogue);
    }
    public virtual void StartRoundEffect(){

    }
    public virtual void StartTurnEffect(bool first){
        /*if(first==true){
            Stats stats=BattleManager.instance.EnemyParty[0].Character.stats;
            if(stats.MaxHP>stats.HP &&!DialogueFlag){
                Battle.CanStartTurn=false;
                Dialog.DialogicRoot.Connect("signal_event",endDialogue);
                DialogueFlag=true;
                Dialog.StartDialogue("BattleTest",false);
            }
        }*/
    }
    public virtual void EndBattleEffect(){

    }
    public virtual void WinEffect(){
        BattleManager.instance.ReturnToOverworld();
    }
    public virtual void LoseEffect(){
        BattleManager.instance.ReturnToOverworld();
    }
    public virtual void RunEffect(){
        BattleManager.instance.ReturnToOverworld();
    }

    public virtual Vector2 GetCenterView(Array<Vector2> Party, Array<Vector2>Enemy){
        float xMin=Mathf.Inf;
        float yMin=Mathf.Inf;
        float xMax=0;
        float yMax=0;
        for(int i=0;i<Party.Count;i++){
            if(Party[i].X>xMax){
                xMax=Party[i].X;
            }
            if(Party[i].Y>yMax){
                yMax=Party[i].Y;
            }
            if(Party[i].X<xMin){
                xMin=Party[i].X;
            }
            if(Party[i].Y<yMin){
                yMin=Party[i].Y;
            }

        }

        for(int i=0;i<Enemy.Count;i++){
            if(Enemy[i].X>xMax){
                xMax=Enemy[i].X;
            }
            if(Enemy[i].Y>yMax){
                yMax=Enemy[i].Y;
            }
            if(Enemy[i].X<xMin){
                xMin=Enemy[i].X;
            }
            if(Enemy[i].Y<yMin){
                yMin=Enemy[i].Y;
            }
        }
        Vector2 Result=new Vector2((xMin+xMax)/2,(yMin+yMax)/2);
        return Result;
    }
    public virtual void EndDialogue(string argument){
        if(argument=="End"){
            Battle.CanStartTurn=true;
            Battle.StartTurn(false);
            Dialog.DialogicRoot.Disconnect("signal_event",endDialogue);
        }
    }
}

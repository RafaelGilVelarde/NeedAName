using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;

[GlobalClass]
public partial class ChangeFlagSimple : BattleScene
{
    [Export] Variant Value;
    [Export] string DictKey;
    [Export] Dictionary<string,Variant> ChangeValue;
    public override void ReturnToOverworld(){
        
        base.ReturnToOverworld();
        BattleManager Battle=BattleManager.instance;
        BattleState State=Battle.State;
        switch (State){
            case BattleState.Win:
                Debug.WriteLine("Changing Flag");
                Flags Aux = GameManager.Instance.Data.Flags;
                //Aux.ChangeDictFlag(DictKey,Value);
                for(int i = 0; i < ChangeValue.Keys.Count; i++)
                {
                    Aux.ChangeDictFlag(ChangeValue.Keys.ElementAt(i),ChangeValue[ChangeValue.Keys.ElementAt(i)]);
                }
            break;
        }
    }
}

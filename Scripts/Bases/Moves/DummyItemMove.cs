using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class DummyItemMove : MoveBase
{
    public override void Effect(Array<BattleCharacter> Users, Array<BattleCharacter> Targets)
    {
        base.Effect(Users, Targets);
        Array<Character> Aux = new Array<Character>();
        for(int i = 0;i<Targets.Count;i++){
            Aux.Add(Targets[i].Character);
        }
        for(int i = 0;i<Users.Count;i++){
            Users[i].CurrentItem.Use(Aux);
            Aux.Add(Targets[i].Character);
        }
    }
}

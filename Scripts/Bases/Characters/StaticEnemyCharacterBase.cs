using Godot;
using System;
[GlobalClass]

public partial class StaticEnemyCharacterBase : EnemyCharacterBase
{
    public override (Vector2,Vector2,int) Moving(EnemyOverworldController controller){
        return(Vector2.Zero,Vector2.Zero,0);
    }
}

using Godot;
using System;

[GlobalClass]
public partial class StatusBase : Resource
{
    [Export] public string Name;
    [Export] public int GameID;
    
    public virtual void Effect(BattleCharacter character)
    {
        
    }
    public virtual void OverworldEffect(OverworldController character)
    {
        
    }
}

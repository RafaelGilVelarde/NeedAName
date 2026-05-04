using Godot;
using Godot.Collections;
using System;
[GlobalClass]

public partial class ItemBase : Resource
{
  public enum Type{
    Consumable,
    Equipment,
    Key,
  }
    [Export] public int ID;
    [Export] public string Name;
    [Export] public string Description;
    [Export] public Type type;
    [Export] public bool HasUse = true;
    public virtual void Effect(Array<Character> Targets){
        
    }
}

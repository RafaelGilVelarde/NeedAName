using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
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
    public virtual void Effect(Array<Character> Targets){
        
    }
}

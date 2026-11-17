using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class EnemyDrops : Resource
{
    [Export] public Array<Items> Items;
    [Export] public Array<int> Chances;
    public Items ChooseDrop()
    {
        RandomNumberGenerator RNG = new RandomNumberGenerator();
        int Result = RNG.RandiRange(0,100); 
        int Return = 0;
        for(int i = 0; i < Chances.Count; i++)
        {
            if(Result<=Chances[i] && Result > Chances[Mathf.Clamp(i - 1, 0, Chances.Count)])
            {
                Return = i;
            }
        }

        return Items[Return];
    }
}

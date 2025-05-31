using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
[GlobalClass]

public partial class PartyCharacterBase : CharacterBase
{
    [Export] public int PartyId;
    [Export] public float ExpSpeed, ExpDistance;
    [Export] public Array<int> ExpForLevel;
    public void SetEXPLevels()
    {
        for (int i = 0; i < 100; i++)
        {
            ExpForLevel[i] = (int)Mathf.Pow(i / ExpSpeed, ExpDistance);
        }
        for (int i = 0; i < 10; i++)
        {
            Debug.WriteLine("Needed EXP: "+ExpForLevel[i]);
        }
    }
}

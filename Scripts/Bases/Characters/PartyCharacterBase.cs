using Godot;
using System;
[GlobalClass]

public partial class PartyCharacterBase : CharacterBase
{
    [Export] public int PartyId;
    [Export] public float ExpSpeed, ExpDistance;
}

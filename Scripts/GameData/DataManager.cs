using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class DataManager : Resource
{
	[Export]public Array<PartyCharacters> Party;
	[Export]public Flags Flags;
}

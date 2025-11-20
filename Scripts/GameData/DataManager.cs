using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class DataManager : Resource
{
	[Export]public Array<PartyCharacters> Party;
	[Export]public Array<Character> CurrentFollowers, AllFollwers;
	[Export]public Array<TypedItemList> items;
	[Export]public Flags Flags;
	[Export] public int Scene, AreaIndex;
	[Export] public Vector2 Position;
	[Export] public int GraphicsLayer;
	[Export] public uint PhysicsLayer;
	[Export] public Array<int> CollisionLayer, CollisionMask;
	[Export] public bool Latest;

	public DataManager DuplicateData()
	{
		DataManager data = (DataManager)Duplicate(true);
		for (int i = 0; i < Party.Count; i++)
		{
			data.Party[i] = (PartyCharacters)Party[i].Duplicate();
			data.Party[i].stats = (Stats)Party[i].stats.Duplicate(true);
			data.Party[i].Equipment = Party[i].Equipment.Duplicate();
			data.Party[i].EquipStats = (Stats)Party[i].EquipStats.Duplicate(true);
			data.Party[i].TotalStats = (Stats)Party[i].TotalStats.Duplicate(true);
		}
		for (int i = 0; i < CurrentFollowers.Count; i++)
		{
			data.CurrentFollowers[i] = (Character)CurrentFollowers[i].Duplicate();
			data.CurrentFollowers[i].stats = (Stats)CurrentFollowers[i].stats.Duplicate(true);
			data.CurrentFollowers[i].Equipment = CurrentFollowers[i].Equipment.Duplicate();
			data.CurrentFollowers[i].EquipStats = (Stats)CurrentFollowers[i].EquipStats.Duplicate(true);
			data.CurrentFollowers[i].TotalStats = (Stats)CurrentFollowers[i].TotalStats.Duplicate(true);
		}
		for (int i = 0; i < items.Count; i++)
		{
			data.items[i] = (TypedItemList)items[i].Duplicate();
			data.items[i] = (TypedItemList)items[i].Duplicate();
		}
		return data;

	}
}

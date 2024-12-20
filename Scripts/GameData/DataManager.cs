using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class DataManager : Resource
{
	[Export]public Array<PartyCharacters> Party;
	[Export]public Array<TypedItemList> items;
	[Export]public Flags Flags;
	[Export] public int Scene, AreaIndex;
	[Export] public Vector2 Position;

	public DataManager DuplicateData(){
		DataManager data = (DataManager)this.Duplicate(true);
		for(int i = 0;i<Party.Count;i++){
			data.Party[i] = (PartyCharacters)Party[i].Duplicate(true);
			data.Party[i].stats= (Stats)Party[i].stats.Duplicate(true);
			data.Party[i].Equipment=Party[i].Equipment.Duplicate(true);
			data.Party[i].EquipStats= (Stats)Party[i].EquipStats.Duplicate(true);
			data.Party[i].TotalStats= (Stats)Party[i].TotalStats.Duplicate(true);
		}		
		for(int i =0;i<items.Count;i++){
			data.items[i]= (TypedItemList)items[i].Duplicate(true);
			data.items[i]= (TypedItemList)items[i].Duplicate(true);
		}
		return data;

	}
}

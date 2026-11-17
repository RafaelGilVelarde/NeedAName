using Godot;
using Godot.Collections;
using System;

public partial class DateManager : Node
{
    public void PrepareEndDay()
	{
		Dictionary<string,Variant> Dict = GameManager.Instance.Data.Flags.Dict;

		Array<Quest> SideQuests = (Array<Quest>)Dict["Sidequests"];
		Array<Quest> MainQuests = (Array<Quest>)Dict["Mainquests"];

		Array<Quest> FinishedSideQuests = new Array<Quest>();
		Array<Quest> FinishedMainQuests = new Array<Quest>();
		
		for(int i = 0; i < SideQuests.Count-1; i++)
		{
			if(SideQuests[i].Completed && !SideQuests[i].TurnedIn)
			{
				FinishedSideQuests.Add(SideQuests[i]);				
			}
		}
		for(int i = 0; i < MainQuests.Count-1; i++)
		{
			if(MainQuests[i].Completed && !MainQuests[i].TurnedIn)
			{
				FinishedMainQuests.Add(MainQuests[i]);				
			}
		}
		FinishedSideQuests[FinishedSideQuests.Count-1]._TurnInQuestEnded+=FinishedMainQuests[0].TurnInQuest;

		for(int i = 0; i < FinishedSideQuests.Count-1; i++)
		{
			FinishedSideQuests[i]._TurnInQuestEnded+=FinishedSideQuests[i+1].TurnInQuest;
		}
		for(int i = 0; i < FinishedMainQuests.Count-1; i++)
		{
			FinishedMainQuests[i]._TurnInQuestEnded+=FinishedMainQuests[i+1].TurnInQuest;
		}


		FinishedMainQuests[FinishedMainQuests.Count-1]._TurnInQuestEnded+=EndDay;
		FinishedSideQuests[0].TurnInQuest();
	}
	public void EndDay()
	{
		Dictionary<string,Variant> Dict = GameManager.Instance.Data.Flags.Dict;

		Array<Quest> SideQuests = (Array<Quest>)Dict["Sidequests"];
		Array<Quest> MainQuests = (Array<Quest>)Dict["Mainquests"];

		Array<Quest> FinishedSideQuests = new Array<Quest>();
		Array<Quest> FinishedMainQuests = new Array<Quest>();
		
		for(int i = 0; i < SideQuests.Count-1; i++)
		{
			if(SideQuests[i].Completed && !SideQuests[i].TurnedIn)
			{
				FinishedSideQuests.Add(SideQuests[i]);				
			}
		}
		for(int i = 0; i < MainQuests.Count-1; i++)
		{
			if(MainQuests[i].Completed && !MainQuests[i].TurnedIn)
			{
				FinishedMainQuests.Add(MainQuests[i]);				
			}
		}
		FinishedSideQuests[FinishedSideQuests.Count-1]._TurnInQuestEnded-=FinishedMainQuests[0].TurnInQuest;

		for(int i = 0; i < FinishedSideQuests.Count-1; i++)
		{
			FinishedSideQuests[i]._TurnInQuestEnded-=FinishedSideQuests[i+1].TurnInQuest;
		}
		for(int i = 0; i < FinishedMainQuests.Count-1; i++)
		{
			FinishedMainQuests[i]._TurnInQuestEnded-=FinishedMainQuests[i+1].TurnInQuest;
		}

		
		FinishedMainQuests[FinishedMainQuests.Count-1]._TurnInQuestEnded-=EndDay;

		GameManager.Instance.Data.Flags.ChangeDictFlag("Date",(int)Dict["Date"]+1);
	}
	public void StartDay()
	{
	}
}

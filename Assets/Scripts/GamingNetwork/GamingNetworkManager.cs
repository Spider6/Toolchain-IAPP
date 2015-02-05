using UnityEngine;
using System.Collections;

public class GamingNetworkManager 
{
	private GamingNetworkAdapterBase adapter;
	
	public void Initialize(RuntimePlatform currentPlatform)
	{
		SetAdapter(currentPlatform);
	}

	public void Authenticate()
	{
		adapter.Init();
	}

	public void ShowAchievements()
	{
		adapter.ShowAchievements();
	}

	private void SetAdapter(RuntimePlatform currentPlatform)
	{
		switch(currentPlatform)
		{
			case RuntimePlatform.Android:
				adapter = new GooglePlayGamesAdapter();
				break;
			case RuntimePlatform.IPhonePlayer:
				adapter = new GameCenterAdapter();
				break;
			default:
				adapter = new DummyAchievementsAdapter(); 
				break;
		}
	}

}

using UnityEngine;
using System.Collections;

public class TestGameAdapter : MonoBehaviour
{
	private GamingNetworkManager manager;

	private void Awake()
	{
		manager = new GamingNetworkManager();
		manager.Initialize(Application.platform);
	}

	public void Authenticate()
	{
		manager.Authenticate();
	}

	public void ShowAchievements()
	{
		manager.ShowAchievements();
	}
}

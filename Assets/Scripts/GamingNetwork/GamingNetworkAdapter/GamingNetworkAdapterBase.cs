using UnityEngine;
using System.Collections.Generic;

public abstract class GamingNetworkAdapterBase 
{
	protected abstract string NetworkType { get; }

	private string ActiveUserId 
	{
		get{ return Social.Active.localUser.id; }
	}

	public bool IsAuthenticated
	{
		get { return Social.Active.localUser.authenticated; }
	}
	
	public virtual void Init ()
	{
		if(!IsAuthenticated)
			StartAuthenticate();
	}
	
	public virtual void ShowAchievements()
	{
		if (IsAuthenticated)
			Social.ShowAchievementsUI();
		else
			StartAuthenticate();
	}

	public abstract void ResetAllAchievements ();
	
	public abstract void AchievementProgressed ();
	
	private void OnAuthenticate (bool wasSuccessful)
	{
		if (wasSuccessful)
			OnSuccessAuthenticate();
		else
			OnFiledAuthenticate();
	}

	public virtual void StartAuthenticate()
	{
		Social.localUser.Authenticate (OnAuthenticate);
	}
	
	private void OnSuccessAuthenticate()
	{
		string userInfo = "Username: " + Social.Active.localUser.userName + "\nUser ID: " + ActiveUserId;
		Debug.Log (NetworkType + ": Authentication successful " + userInfo);
	}
	
	protected virtual void OnFiledAuthenticate()
	{
		Debug.Log (NetworkType + ": Authentication failed");
	}
}

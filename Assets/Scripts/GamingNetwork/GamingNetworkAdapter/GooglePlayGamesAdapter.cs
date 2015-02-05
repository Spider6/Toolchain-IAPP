using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames;

public class GooglePlayGamesAdapter : GamingNetworkAdapterBase
{
	private PlayGamesPlatform GooglePlayPlatformactive
	{
		get { return (PlayGamesPlatform)Social.Active; }
	}

	protected override string NetworkType 
	{
		get { return "GooglePlayGames"; }
	}

	public override void ResetAllAchievements ()
	{
		Debug.LogWarning ("GooglePlayGames: Reset achievements not implemented, see https://developers.google.com/games/services/management/api/achievements/reset");
	}

	private void HandleGooglePlayUnlockResponse(bool wasSuccessful)
	{
		if (wasSuccessful)
			Debug.Log  ("GooglePlayGames: Achievement unlocked.");
		else
			Debug.Log ("GooglePlayGames: Achievement was not unlocked.");
	}

	public override void StartAuthenticate ()
	{
		PlayGamesPlatform.Activate();
		base.StartAuthenticate ();
	}

	public override void AchievementProgressed ()
	{
		//double percent = ((double)achievement.Progress * 100) / (double)achievement.Goal;

//		if (percent == 100 || percent == 0)
//			Social.ReportProgress (achievement.Id, percent, HandleGooglePlayUnlockResponse);
//		else
//			GooglePlayPlatform.IncrementAchievement (achievement.Id, (int)percent, HandleGooglePlayUnlockResponse);
	}
}

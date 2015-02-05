using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.GameCenter;

public class GameCenterAdapter : GamingNetworkAdapterBase
{
	protected override string NetworkType 
	{
		get { return "GameCenter"; }
	}

	public override void ResetAllAchievements ()
	{
		GameCenterPlatform.ResetAllAchievements( (resetResult) => {
			Debug.Log( "GameCenter: " + ((resetResult) ? "Reset done." : "Reset failed."));
		});
	}

	private void OnRegistered (bool result)
	{
		if (result)
			Debug.Log ("GameCenter: Successfully reported progress");
		else
			Debug.Log ("GameCenter: Failed to report progress");
	}

	public override void AchievementProgressed ()
	{
		if(IsAuthenticated)
		{
//			double percent = ((double)achievement.Progress * 100) / (double)achievement.Goal;
//
//			IAchievement a = Social.CreateAchievement();
//			a.id = achievement.Id;
//			a.percentCompleted = percent;
//			a.ReportProgress(OnRegistered);		
		}
	}
}

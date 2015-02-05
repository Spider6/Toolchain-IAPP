
public class DummyAchievementsAdapter : GamingNetworkAdapterBase 
{
	protected override string NetworkType 
	{
		get { return string.Empty; }
	}

	public override void ResetAllAchievements ()
	{
	}

	public override void AchievementProgressed ()
	{
	}
}
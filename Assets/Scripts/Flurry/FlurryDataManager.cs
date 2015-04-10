using UnityEngine;
using System.Collections;

public class FlurryDataManager
{
	private static FlurryDataManager instance;
	
	public static FlurryDataManager Instance
	{
		get
		{
			if (instance == null)
				instance = new FlurryDataManager ();
			
			return instance;
		}
	}

	public void Initialize()
	{
		FlurryAndroid.onStartSession("874B6ZGZS2RRJBH4DPFH", false, false);
	}

}

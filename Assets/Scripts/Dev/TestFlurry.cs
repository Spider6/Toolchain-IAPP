using UnityEngine;
using System.Collections;

public class TestFlurry : MonoBehaviour 
{
	private void Awake () 
	{
		FlurryDataManager.Instance.Initialize();
	}
}

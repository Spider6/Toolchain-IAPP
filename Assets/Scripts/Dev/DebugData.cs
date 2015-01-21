using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugData : MonoBehaviour 
{
	[SerializeField]
	private StoreData simulateStore;

	[SerializeField]
	private List<IAPProduct> debugProducts;

	[SerializeField]
	private string googlePublicKey;

	[SerializeField]
	private float timeOutToStore;

	[SerializeField]
	private float storeDebugDelayInSeconds;

	public StoreData SimulateStore 
	{
		get { return simulateStore; }
	}

	public List<IAPProduct> DebugProducts
	{
		get { return debugProducts;	}
	}

	public string GooglePublicKey 
	{
		get { return googlePublicKey; }
	}

	public float TimeOutToStore 
	{
		get { return timeOutToStore; }
	}

	public float StoreDebugDelayInSeconds 
	{
		get { return storeDebugDelayInSeconds; }
	}
}

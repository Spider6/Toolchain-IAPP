using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugData : MonoBehaviour 
{
	[SerializeField]
	private StoreData simulateStore;

	[SerializeField]
	private List<IAPProduct> debugProducts;

	public StoreData SimulateStore 
	{
		get { return simulateStore; }
	}

	public List<IAPProduct> DebugProducts
	{
		get { return debugProducts;	}
	}
}

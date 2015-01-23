using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IAP;

namespace IAP.Detail
{
	public class DebugStoreData : ScriptableObject, IDebugStoreData
	{
		[SerializeField]
		private StoreData simulateStore;

		[SerializeField]
		private List<IAPProduct> debugProducts;

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

		public float StoreDebugDelayInSeconds 
		{
			get { return storeDebugDelayInSeconds; }
		}
	}
}